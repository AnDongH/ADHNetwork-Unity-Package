using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol {

    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class ProtocolReq {
        public string Path { get; protected set; }

        public ProtocolID ProtocolID { get; protected set; }

    }

}
