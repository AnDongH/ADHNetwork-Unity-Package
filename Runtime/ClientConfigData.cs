using System.Collections;
using System.Collections.Generic;
using ADHNetworkShared.Util;
using Microsoft.Extensions.Configuration;

public class ClientConfigData : ConfigData {
    
    public string ServerUri { get; private set; }
    
    public override void SetConfig(IConfigurationRoot config) {
        ServerUri = config["ServerUri"];
    }

}
