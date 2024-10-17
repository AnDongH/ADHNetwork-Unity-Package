using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol {

    [MemoryPackable(GenerateType.NoGenerate)]
    [Serializable]
    public abstract partial class ProtocolReq {

        [MemoryPackOrder(0)]
        public string Path { get; protected set; }

        [MemoryPackOrder(1)]
        public ProtocolID ProtocolID { get; protected set; }

    }

}
