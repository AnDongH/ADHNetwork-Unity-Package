using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol {

    [MemoryPackable]
    [Serializable]
    public partial class PostTestReq : ProtocolReq {

        [MemoryPackOrder(2)]
        public string content = "hello";

        [MemoryPackConstructor]
        public PostTestReq(string path, string content) : base() {
            Path = path;
            this.content = content;
            ProtocolID = ProtocolID.PostTest;
        }

    }
}
