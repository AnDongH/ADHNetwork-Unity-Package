using MemoryPack;

namespace ADHNetworkShared.Protocol.DTO {

    public enum ProtocolID {

        Handshake,
        PostTest,
        AuthPostTest,
        Login,
        LoginAuth,
        Logout,
        CreateAccount,
        DeleteAccount,
        UserInfo,
        GetAttendance,
        SetAttendance,
        CheckAttendance,
        RewardAttendance,
        Ping,
        Mail,
        RewardMail,
        DeleteMail,
        Dummy1List,
        Dummy2List,
        Dummy3List,
        FriendInfo,
        FriendReqInfo,
        FriendRcvInfo,
        FriendReq,
        FriendAccept,
        FriendDelete,
        FriendCancel,
        FriendDeny,
        SetScore,
        GetMyRanking,
        GetAllRanking

    }


    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class ProtocolReq {
        
        public ProtocolID protocolID { get; set; }
    }

    [MemoryPackable]
    public partial class AuthProtocolReq : ProtocolReq {
        public string AuthToken { get; set; }
        public long UserID { get; set; }

        public AuthProtocolReq(string authToken, long userid) {
            AuthToken = authToken;
            UserID = userid;
        }
    }

}
