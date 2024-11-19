using ADHNetworkShared.Protocol.DTO;
using MemoryPack;
using MemoryPack.Formatters;


namespace ADHNetworkShared.Protocol {

    [MemoryPackUnionFormatter(typeof(ProtocolReq))]
    [MemoryPackUnion(0, typeof(AuthProtocolReq))]
    [MemoryPackUnion(1, typeof(DtoLoginReq))]
    [MemoryPackUnion(2, typeof(DtoLogoutReq))]
    [MemoryPackUnion(3, typeof(DtoLoginAuthReq))]
    [MemoryPackUnion(4, typeof(DtoAttendanceGetReq))]
    [MemoryPackUnion(5, typeof(DtoAccountRegisterReq))]
    [MemoryPackUnion(6, typeof(DtoAccountDeleteReq))]
    [MemoryPackUnion(7, typeof(PostTestReq))]
    [MemoryPackUnion(8, typeof(DtoHandShakeReq))]
    [MemoryPackUnion(9, typeof(AuthPostTestReq))]
    [MemoryPackUnion(10, typeof(DtoAttendanceSetReq))]
    [MemoryPackUnion(11, typeof(DtoAttendanceRewardReq))]
    [MemoryPackUnion(12, typeof(DtoAttendanceCheckReq))]
    [MemoryPackUnion(13, typeof(PingReq))]
    [MemoryPackUnion(14, typeof(DtoMailListReq))]
    [MemoryPackUnion(15, typeof(DtoMailRewardReq))]
    [MemoryPackUnion(16, typeof(DtoMailDeleteReq))]
    [MemoryPackUnion(17, typeof(DtoUserDummy1InfosReq))]
    [MemoryPackUnion(18, typeof(DtoUserDummy2InfosReq))]
    [MemoryPackUnion(19, typeof(DtoUserDummy3InfosReq))]
    [MemoryPackUnion(20, typeof(DtoFriendInfoListReq))]
    [MemoryPackUnion(21, typeof(DtoFriendReqInfoListReq))]
    [MemoryPackUnion(22, typeof(DtoFriendReceivedInfoListReq))]
    [MemoryPackUnion(23, typeof(DtoFriendReqReq))]
    [MemoryPackUnion(24, typeof(DtoFriendAcceptReq))]
    [MemoryPackUnion(25, typeof(DtoFriendDeleteReq))]
    [MemoryPackUnion(26, typeof(DtoFriendReqCancelReq))]
    [MemoryPackUnion(27, typeof(DtoFriendReqDenyReq))]
    [MemoryPackUnion(28, typeof(DtoAccountInfoReq))]
    [MemoryPackUnion(29, typeof(DtoScoreSetReq))]
    [MemoryPackUnion(30, typeof(DtoGetMyRankingReq))]
    [MemoryPackUnion(31, typeof(DtoGetAllRankingReq))]
    public partial class UnionProtocolReqFormatter {

    }

    [MemoryPackUnionFormatter(typeof(ProtocolRes))]
    [MemoryPackUnion(0, typeof(DtoLoginRes))]
    [MemoryPackUnion(1, typeof(BasicProtocolRes))]
    [MemoryPackUnion(2, typeof(DtoAttendanceGetRes))]
    [MemoryPackUnion(3, typeof(PostTestRes))]
    [MemoryPackUnion(4, typeof(DtoHandShakeRes))]
    [MemoryPackUnion(5, typeof(DtoRewardRes))]
    [MemoryPackUnion(6, typeof(DtoAttendanceCheckRes))]
    [MemoryPackUnion(7, typeof(DtoMailListRes))]
    [MemoryPackUnion(8, typeof(DtoUserItemInfosRes))]
    [MemoryPackUnion(9, typeof(DtoFriendInfoListRes))]
    [MemoryPackUnion(10, typeof(DtoGetMyRankingRes))]
    [MemoryPackUnion(11, typeof(DtoGetAllRankingRes))]
    public partial class UnionProtocolResFormatter {

    }

    [MemoryPackUnionFormatter(typeof(ItemInfo))]
    [MemoryPackUnion(0, typeof(Dummy1ItemInfo))]
    [MemoryPackUnion(1, typeof(Dummy2ItemInfo))]
    [MemoryPackUnion(2, typeof(Dummy3ItemInfo))]
    public partial class UnionItemInfoFormatter {

    }

}
