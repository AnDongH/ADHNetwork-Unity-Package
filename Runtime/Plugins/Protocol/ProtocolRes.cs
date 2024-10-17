using System;
using System.Collections.Generic;
using System.Text;
using MemoryPack;

namespace ADHNetworkShared.Protocol {

    [MemoryPackable]
    [MemoryPackUnion(0, typeof(PostTestRes))]
    public abstract partial class ProtocolRes {

    }

}
