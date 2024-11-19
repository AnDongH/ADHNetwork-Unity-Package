using ADHNetworkShared.Crypto;
using MemoryPack;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Tls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RSATest : MonoBehaviour
{
    [Header("평문, 암호문")]
    [SerializeField] private string plainText;
    [SerializeField] private byte[] cipherbyte;

    [Header("개인키, 공개키")]
    [SerializeField] private byte[] privateKey;
    [SerializeField] private byte[] publicKey;
    [SerializeField] private string publicKeyString;
    [SerializeField] private string privateKeyString;
    [SerializeField] private byte[] testPublicKeyByte;
    [SerializeField] private byte[] testPrivateKeyByte;

    [Header("복호 결과")]
    [SerializeField] private string result;

    private AsymmetricCipherKeyPair pair;

    public void Encrypt() {

        pair = ECIES.GenerateECIESKeyPair();

        publicKey = (pair.Public as ECPublicKeyParameters).Q.GetEncoded();
        privateKey = (pair.Private as ECPrivateKeyParameters).D.ToByteArray();

        publicKeyString = Convert.ToBase64String(publicKey);
        testPublicKeyByte = Convert.FromBase64String(publicKeyString);

        privateKeyString = Convert.ToBase64String(privateKey);
        testPrivateKeyByte = Convert.FromBase64String(privateKeyString);

        byte[] plainByte = MemoryPackSerializer.Serialize(plainText);

        cipherbyte = ECIES.EncryptECIES(plainByte, ECIES.RestorePublicBytesToKey(testPublicKeyByte), ECIES.GenerateECIESKeyPair());

        plainByte = ECIES.DecryptECIES(cipherbyte, ECIES.RestorePrivateBytesToKey(privateKey));

        result = MemoryPackSerializer.Deserialize<string>(plainByte);
    }

}
