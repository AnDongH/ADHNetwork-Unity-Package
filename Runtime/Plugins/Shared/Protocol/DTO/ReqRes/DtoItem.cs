using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoItemReq : AuthProtocolReq {
        public DtoItemReq(string authToken, long userid) : base(authToken, userid) {

        }

    }


    [MemoryPackable]
    public partial class DtoUserDummy1InfosReq : DtoItemReq {

        public DtoUserDummy1InfosReq(string authToken, long userid) : base(authToken, userid) {
        
            protocolID = ProtocolID.Dummy1List;

        }
    
    }

    [MemoryPackable]
    public partial class DtoUserDummy2InfosReq : DtoItemReq {

        public DtoUserDummy2InfosReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.Dummy2List;

        }

    }

    [MemoryPackable]
    public partial class DtoUserDummy3InfosReq : DtoItemReq {

        public DtoUserDummy3InfosReq(string authToken, long userid) : base(authToken, userid) {

            protocolID = ProtocolID.Dummy3List;

        }

    }

    [MemoryPackable]
    public partial class DtoUserItemInfosRes : ProtocolRes {
        public List<(ItemInfo, int)> userItemInfos { get; set; }

    }

}
