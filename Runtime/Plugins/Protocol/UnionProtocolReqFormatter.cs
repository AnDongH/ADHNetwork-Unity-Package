using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using MemoryPack;
using MemoryPack.Formatters;


namespace ADHNetworkShared.Protocol {

    public static class UnionProtocolFormatter {
        
        public static void Register() {
            
            var req = new DynamicUnionFormatter<ProtocolReq>(
            (0, typeof(PostTestReq)));

            var res = new DynamicUnionFormatter<ProtocolRes>(
            (0, typeof(PostTestRes)));

            MemoryPackFormatterProvider.Register(req);
            MemoryPackFormatterProvider.Register(res);
        }

    }

}
