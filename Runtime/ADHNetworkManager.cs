using ADHNetworkShared.Crypto;
using ADHNetworkShared.Protocol;
using Cysharp.Threading.Tasks;
using MemoryPack;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEngine;

public static class ADHNetworkManager {
    public static string ServerUrl { get; private set; } = "http://10.10.3.2:7777";
    public static HttpClient Client { get; private set; } = new HttpClient();

    private static Dictionary<ProtocolID, IProtocolHandler> handlerMap = new Dictionary<ProtocolID, IProtocolHandler>()
    { { ProtocolID.PostTest, new PostTestHandler() }, };

    public static async UniTask HandShake() {

        try {

            var clientKeyPair = DiffieHellman.GenerateECKeyPair();
            var clientPrivateKey = clientKeyPair.Private as ECPrivateKeyParameters;
            var clientPublicKey = clientKeyPair.Public as ECPublicKeyParameters;

            using (HttpResponseMessage res = await Client.PostAsync($"{ServerUrl}/handshake", new ByteArrayContent(clientPublicKey.Q.GetEncoded()))) {

                byte[] serverPublicKeyCode = await res.Content.ReadAsByteArrayAsync();

                AES.key = DiffieHellman.GenerateSharedSecret(clientPrivateKey, DiffieHellman.RestorePublicBytesToKey(serverPublicKeyCode));

                Debug.Log(BitConverter.ToString(AES.key));
                Debug.Log(AES.key.Length);
            }

        } catch (Exception ex) {

            Debug.LogException(ex);

        }

    }

    public static async UniTask GetRequestAsync(ProtocolReq req) {

        try {

            using (HttpResponseMessage m = await Client.GetAsync($"{ServerUrl}{req.Path}"))
            using (Stream st = await m.Content.ReadAsStreamAsync()) {
                EncryptedData data = await MemoryPackSerializer.DeserializeAsync<EncryptedData>(st);
                byte[] decryptedData = AES.DecryptAES(data.Data, data.IV);
                ProtocolRes res = MemoryPackSerializer.Deserialize<ProtocolRes>(decryptedData);
                handlerMap[req.ProtocolID].Process(res);
            }

        } catch (Exception ex) {
            Debug.LogException(ex);
        }

    }

    public static async UniTask PostRequestAsync(ProtocolReq req) {

        try {

            //UnionProtocolFormatter.Register();
            UnionProtocolReqFormatterInitializer.RegisterFormatter();
            UnionProtocolResFormatterInitializer.RegisterFormatter();

            byte[] reqBytes = MemoryPackSerializer.Serialize(req);
            (byte[] encryptedReq, byte[] iv) = AES.EncryptAES(reqBytes);

            EncryptedData encryptedData = new EncryptedData(encryptedReq, iv);
            byte[] encryptedDataBytes = MemoryPackSerializer.Serialize(encryptedData);

            using (HttpResponseMessage m = await Client.PostAsync($"{ServerUrl}{req.Path}", new ByteArrayContent(encryptedDataBytes))) {
                byte[] mb = await m.Content.ReadAsByteArrayAsync();
                EncryptedData data = MemoryPackSerializer.Deserialize<EncryptedData>(mb);
                byte[] decryptedData = AES.DecryptAES(data.Data, data.IV);
                ProtocolRes res = MemoryPackSerializer.Deserialize<ProtocolRes>(decryptedData);

                handlerMap[req.ProtocolID].Process(res);
            }
        } catch (Exception ex) {
            Debug.LogException(ex);
        }

    }

}
