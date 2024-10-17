using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol {

    [MemoryPackable]
    public partial class PostTestRes : ProtocolRes {

        public string content = "";

        public PostTestRes(string content) : base() { 
            this.content = content;
        }
    }

}
