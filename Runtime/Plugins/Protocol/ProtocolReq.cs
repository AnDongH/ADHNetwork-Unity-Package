using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol {

    [MemoryPackable]
    [MemoryPackUnion(0, typeof(PostTestReq))]
    public abstract partial class ProtocolReq {
        public string Path { get; protected set; }

        public ProtocolID ProtocolID { get; protected set; }

    }

}
