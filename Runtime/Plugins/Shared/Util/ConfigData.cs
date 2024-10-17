using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Util {
    public abstract class ConfigData {

        public abstract void SetConfig(IConfigurationRoot config);
        
    }
}
