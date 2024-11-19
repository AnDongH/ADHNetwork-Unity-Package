using System;
using System.Collections.Generic;
using ADHNetworkShared.Protocol.DTO;

namespace ADHNetworkShared.Protocol {
    public static class Router {

        private static Dictionary<ProtocolID, string> routingMap = new Dictionary<ProtocolID, string>()
        {   
            { ProtocolID.Handshake, "/noneAuth/handshake" },
            { ProtocolID.PostTest, "/noneAuth/posttest" },
            { ProtocolID.AuthPostTest, "/test/authposttest" },
            { ProtocolID.CreateAccount, "/noneAuth/account/create"},
            { ProtocolID.DeleteAccount, "/account/delete"},
            { ProtocolID.UserInfo, "/account/info"},
            { ProtocolID.Login, "/noneAuth/login"},
            { ProtocolID.Logout, "/logout"},
            { ProtocolID.GetAttendance, "/attendance/getinfo"}, 
            { ProtocolID.SetAttendance, "/attendance/setinfo"}, 
            { ProtocolID.CheckAttendance, "/attendance/check"}, 
            { ProtocolID.RewardAttendance, "/attendance/reward"}, 
            { ProtocolID.Ping, "/ping/ping"}, 
            { ProtocolID.Mail, "/mail/get"},
            { ProtocolID.RewardMail, "/mail/reward"}, 
            { ProtocolID.DeleteMail, "/mail/delete"}, 
            { ProtocolID.Dummy1List, "/item/dummy1/list"},
            { ProtocolID.Dummy2List, "/item/dummy2/list"},
            { ProtocolID.Dummy3List, "/item/dummy3/list"},
            { ProtocolID.FriendInfo, "/friend/info"},
            { ProtocolID.FriendReqInfo, "/friend/reqinfo"},
            { ProtocolID.FriendRcvInfo, "/friend/rcvinfo"},
            { ProtocolID.FriendReq, "/friend/req"},
            { ProtocolID.FriendAccept, "/friend/acc"},
            { ProtocolID.FriendDelete, "/friend/del"},
            { ProtocolID.FriendCancel, "/friend/cancel"},
            { ProtocolID.FriendDeny, "/friend/deny"},
            { ProtocolID.SetScore, "/score/set"},
            { ProtocolID.GetMyRanking, "/score/get/my"},
            { ProtocolID.GetAllRanking, "/score/get/all"},
            

        };


        public static Dictionary<ProtocolID, string> RoutingMap => routingMap;
        
    }

}
