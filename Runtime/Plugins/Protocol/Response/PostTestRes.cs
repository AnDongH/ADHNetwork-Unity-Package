using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol {

    [MemoryPackable]
    [Serializable]
    public partial class PostTestRes : ProtocolRes {

        [MemoryPackOrder(0)]
        public string content = "";

        [MemoryPackConstructor]
        public PostTestRes(string content) : base() {
            this.content = content;
        }
    }

}
