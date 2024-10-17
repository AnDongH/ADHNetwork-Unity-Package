using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using MemoryPack;


namespace ADHNetworkShared.Protocol {

    [MemoryPackUnionFormatter(typeof(ProtocolReq))]
    [MemoryPackUnion(0, typeof(PostTestReq))]
    public partial class UnionProtocolReqFormatter {

    }

    [MemoryPackUnionFormatter(typeof(ProtocolRes))]
    [MemoryPackUnion(0, typeof(PostTestRes))]
    public partial class UnionProtocolResFormatter {

    }


}
