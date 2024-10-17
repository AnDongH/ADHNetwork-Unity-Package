using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol {

    [MemoryPackable]
    public partial class PostTestReq : ProtocolReq {

        public string content = "hello";

        public PostTestReq(string path, string content) : base() {
            Path = path;
            this.content = content;
            ProtocolID = ProtocolID.PostTest;
        }

    }
}
