using UnityEngine;
using ADHNetworkShared.Protocol.DTO;
using ADHNetworkShared.Util;
using Microsoft.Extensions.Configuration;
using ADHNetworkShared.Protocol;

public static class NetworkSetting
{



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void SetNetwork() {

        UnionProtocolReqFormatterInitializer.RegisterFormatter();
        UnionProtocolResFormatterInitializer.RegisterFormatter();
        UnionItemInfoFormatterInitializer.RegisterFormatter();

        Config.Load("ClientConfig.json", Application.streamingAssetsPath);
    }
    

}
