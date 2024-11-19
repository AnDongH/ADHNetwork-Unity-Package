using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using System.Security.Cryptography;
using System;
using System.Threading;
using ADHNetworkShared.Protocol.DTO;

public static class PlayerClient
{
    
    public static readonly HttpClient Client = new HttpClient();

    public const int RetryCnt = 3;
    public const double TimeOut = 15;

    public static long UID { get; set; }
    public static string AuthToken { get; set; }
    public static byte[] AESKey { get; set; }
    public static byte[] ServerCommonKey { get; set; }
    
    
    public static void Reset() {
        
        UID = default;
        AuthToken = default;
        AESKey = null;
        ServerCommonKey = null;

    }

}
