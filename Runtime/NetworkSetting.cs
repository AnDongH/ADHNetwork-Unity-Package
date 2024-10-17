using UnityEngine;
using ADHNetworkShared.Protocol;
using ADHNetworkShared.Util;
using Microsoft.Extensions.Configuration;

public static class NetworkSetting
{
    public static ClientConfigData configData = new ClientConfigData();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void SetNetwork() {

        UnionProtocolReqFormatterInitializer.RegisterFormatter();
        UnionProtocolResFormatterInitializer.RegisterFormatter();

        Config.Load("ClientConfig.json", configData);

    }
    

}
