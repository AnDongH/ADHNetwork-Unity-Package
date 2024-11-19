using MemoryPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADHNetworkShared.Protocol.DTO {

    [MemoryPackable]
    public partial class DtoAttendanceRewardReq : AuthProtocolReq {
        
        public int day { get; set; }

        public DtoAttendanceRewardReq(string authToken, long userid, int day) : base(authToken, userid) {

            protocolID = ProtocolID.RewardAttendance;
            this.day = day;

        }
    }

    [MemoryPackable]
    public partial class DtoMailRewardReq : AuthProtocolReq {

        public int mail_id { get; set; }

        public DtoMailRewardReq(string authToken, long userid, int mail_id) : base(authToken, userid) {

            protocolID = ProtocolID.RewardMail;
            this.mail_id = mail_id;

        }

    }


    [MemoryPackable]
    public partial class DtoRewardRes : ProtocolRes {
        public List<(ItemInfo, int)> rewardInfos { get; set; }

    }

}
