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
    public static HttpClient Client { get; private set; } = new HttpClient();

    private static Dictionary<ProtocolID, IProtocolHandler> handlerMap = new Dictionary<ProtocolID, IProtocolHandler>()
    { { ProtocolID.PostTest, new PostTestHandler() }, };

    public static async UniTask HandShake() {

        try {

            var clientKeyPair = DiffieHellman.GenerateECKeyPair();
            var clientPrivateKey = clientKeyPair.Private as ECPrivateKeyParameters;
            var clientPublicKey = clientKeyPair.Public as ECPublicKeyParameters;

            using (HttpResponseMessage res = await Client.PostAsync($"{NetworkSetting.configData.ServerUri}/handshake", new ByteArrayContent(clientPublicKey.Q.GetEncoded()))) {
            
                AES.key = DiffieHellman.GenerateSharedSecret(clientPrivateKey, DiffieHellman.RestorePublicBytesToKey(await res.Content.ReadAsByteArrayAsync()));
                
            }

            Debug.Log("handshake: Success key exchange!");

        } catch (Exception ex) {

            Debug.LogException(ex);

        }

    }

    public static async UniTask GetRequestAsync(ProtocolReq req) {

        try {

            using (HttpResponseMessage m = await Client.GetAsync($"{NetworkSetting.configData.ServerUri}{req.Path}"))
            using (Stream st = await m.Content.ReadAsStreamAsync()) {

                EncryptedData data = await MemoryPackSerializer.DeserializeAsync<EncryptedData>(st);
                ProtocolRes res = MemoryPackSerializer.Deserialize<ProtocolRes>(AES.DecryptAES(data.Data, data.IV));
                handlerMap[req.ProtocolID].Process(res);
                
            }

            
        } catch (Exception ex) {
            Debug.LogException(ex);
        }

    }

    public static async UniTask PostRequestAsync(ProtocolReq req) {

        try {

            (byte[] encryptedReq, byte[] iv) = AES.EncryptAES(MemoryPackSerializer.Serialize(req));

            using (HttpResponseMessage m = await Client.PostAsync($"{NetworkSetting.configData.ServerUri}{req.Path}", new ByteArrayContent(MemoryPackSerializer.Serialize(new EncryptedData(encryptedReq, iv))))) {
            
                EncryptedData data = MemoryPackSerializer.Deserialize<EncryptedData>(await m.Content.ReadAsByteArrayAsync());
                ProtocolRes res = MemoryPackSerializer.Deserialize<ProtocolRes>(AES.DecryptAES(data.Data, data.IV));
                handlerMap[req.ProtocolID].Process(res);
            
            }
        } catch (Exception ex) {
            Debug.LogException(ex);
        }

    }

}
