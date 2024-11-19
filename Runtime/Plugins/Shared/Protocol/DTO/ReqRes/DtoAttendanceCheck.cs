using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoAttendanceCheckReq : AuthProtocolReq {
        public DtoAttendanceCheckReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.CheckAttendance;

        }
    
    }

    [MemoryPackable]
    public partial class DtoAttendanceCheckRes : ProtocolRes {

        // T1 : day, (T2 : ItemInfo, T3 : cnt)
        public List<(int, List<(ItemInfo, int)>)> rewards { get; set; }


    }

}
