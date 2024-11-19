using ADHNetworkShared.Crypto;
using ADHNetworkShared.Protocol;
using Cysharp.Threading.Tasks;
using MemoryPack;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using ADHNetworkShared.Protocol.DTO;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEngine;
using ADHNetworkShared.Shared.Util;
using System.Threading;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public static class ADHNetworkManager {

    // CancellationTokenSource는 Thread safe 이기에 로그인에 사용한 걸 다른데서 cancel 시킬 때는 공유 자원으로 한다..
    private static CancellationTokenSource pingCts = null;

    // 공유 자원을 여러 쓰레드에서 사용하는 것이니 락을 걸어준다.
    private static object pingLock = new object();

    #region common method

    private static async UniTask<ProtocolRes> GetRequestAsync(ProtocolReq req) {

        using (HttpResponseMessage m = await PlayerClient.Client.GetAsync($"{ConfigData._config["ServerUrl"]}{Router.RoutingMap[req.protocolID]}"))
        using (Stream st = await m.Content.ReadAsStreamAsync()) {

            EncryptedData data = await MemoryPackSerializer.DeserializeAsync<EncryptedData>(st);
            ProtocolRes res = MemoryPackSerializer.Deserialize<ProtocolRes>(AES.DecryptAES(data.Data, PlayerClient.AESKey, data.IV));

            Debug.Log(m.ToString());

            return res;
        }

    }
    private static async UniTask<ProtocolRes> PostRequestAsync(ProtocolReq req) {

        if (PlayerClient.AESKey == null) {
            Debug.LogError("error: require login auth!!");
            return null;
        }

        (byte[] encryptedReq, byte[] iv) = AES.EncryptAES(MemoryPackSerializer.Serialize(req), PlayerClient.AESKey);

        using (HttpResponseMessage m = await PlayerClient.Client.PostAsync($"{ConfigData._config["ServerUrl"]}{Router.RoutingMap[req.protocolID]}", new ByteArrayContent(MemoryPackSerializer.Serialize(new ClientEncryptedData(encryptedReq, iv, PlayerClient.UID))))) {

            EncryptedData data = MemoryPackSerializer.Deserialize<EncryptedData>(await m.Content.ReadAsByteArrayAsync());
            
            ProtocolRes res = MemoryPackSerializer.Deserialize<ProtocolRes>(AES.DecryptAES(data.Data, PlayerClient.AESKey, data.IV));

            Debug.Log(m.ToString());

            return res;
        }

    }
    private static async UniTask<ProtocolRes> PostNoneAuthRequestAsync(ProtocolReq req) {

        var pair = ECIES.GenerateECIESKeyPair();

        byte[] data = ECIES.EncryptECIES(MemoryPackSerializer.Serialize(req), ECIES.RestorePublicBytesToKey(PlayerClient.ServerCommonKey), pair);

        using (HttpResponseMessage m = await PlayerClient.Client.PostAsync($"{ConfigData._config["ServerUrl"]}{Router.RoutingMap[req.protocolID]}", new ByteArrayContent(data))) {

            byte[] plainData = ECIES.DecryptECIES(await m.Content.ReadAsByteArrayAsync(), pair.Private as ECPrivateKeyParameters);

            ProtocolRes res = MemoryPackSerializer.Deserialize<ProtocolRes>(plainData);

            Debug.Log(m.ToString());

            return res;
        }

    }
    private static bool TryParseResponse<T>(ProtocolRes res, out T targetType) where T : ProtocolRes {

        bool flag = res != null && res is T && res.Result == ErrorCode.None;

        if (flag) {
            targetType = res as T;
            Debug.Log(res.ToString());
        } else {
            targetType = null;
            Debug.LogError(res.ToString());
        }

        return flag;

    }
    private static async UniTask Ping() {

        Debug.Log("핑 시작");
        
        while (true) {

            await UniTask.Delay(TimeSpan.FromMinutes(150));

            try {

                var baseRes = await PostNoneAuthRequestAsync(new PingReq(PlayerClient.AuthToken, PlayerClient.UID));

            } catch (Exception ex) {

                Debug.LogException(ex);
                Debug.Log(ex.ToString());


                return;
            }

        }

    }

    #endregion

    #region inner method

    private static async UniTask<DtoHandShakeRes> HandShakeInner() {

        if (PlayerClient.ServerCommonKey != null) return null;

        ProtocolReq req = new DtoHandShakeReq() { Version = ConfigData._config["Version"] };

        using (HttpResponseMessage res = await PlayerClient.Client.PostAsync($"{ConfigData._config["ServerUrl"]}{Router.RoutingMap[req.protocolID]}", new ByteArrayContent(MemoryPackSerializer.Serialize(req)))) {

            byte[] buffer = await res.Content.ReadAsByteArrayAsync();
            DtoHandShakeRes handRes = MemoryPackSerializer.Deserialize<ProtocolRes>(buffer) as DtoHandShakeRes;

            if (handRes.VersionOK) PlayerClient.ServerCommonKey = handRes.ServerCommonKey;
            else {
                Debug.Log(res.ToString());
                Debug.LogError(handRes.ToString());
                return null;
            }

            Debug.Log("handshake: Success get temp key!");
            return handRes;

        }

    }
    private static async UniTask<DtoHandShakeRes> HandShake(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {
            
            return await HandShakeInner()
                        .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await HandShake(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {
            Debug.LogException(ex);
            Debug.Log(ex.ToString());
        }
        
        return null;

    }
    private static async UniTask<DtoLoginRes> LoginRequest(string id, string pw, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        var clientKeyPair = ECDH.GenerateECKeyPair();
        var clientPrivateKey = clientKeyPair.Private as ECPrivateKeyParameters;
        var clientPublicKey = clientKeyPair.Public as ECPublicKeyParameters;

        try {

            var baseRes = await PostNoneAuthRequestAsync(new DtoLoginReq(id, pw, clientPublicKey.Q.GetEncoded()))
                               .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoLoginRes>(baseRes, out var res)) {

                PlayerClient.UID = res.Uid;
                PlayerClient.AuthToken = res.AuthToken;
                PlayerClient.AESKey = ECDH.GenerateSharedSecret(clientPrivateKey, ECDH.RestorePublicBytesToKey(res.ServerPublicKey));


                lock (pingLock) {

                    if (pingCts != null) {
                        pingCts.Cancel();
                        pingCts.Dispose();
                        pingCts = null;
                    }

                    pingCts = new CancellationTokenSource();
                }

                _ = Ping().WithCancellation(pingCts.Token);

                return res;

            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await LoginRequest(id, pw, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {
            Debug.LogException(ex);
            Debug.Log(ex.ToString());
        }


        return null;
    }
    private static async UniTask<BasicProtocolRes> CreateAccountRequest(string id, string pw, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostNoneAuthRequestAsync(new DtoAccountRegisterReq(id, pw))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await CreateAccountRequest(id, pw, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {
            Debug.LogException(ex);
            Debug.Log(ex.ToString());
        }

        return null;

    }
    private static async UniTask<PostTestRes> PostTest(string msg, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostNoneAuthRequestAsync(new PostTestReq(msg))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<PostTestRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await PostTest(msg, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {
            Debug.LogException(ex);
            Debug.Log(ex.ToString());
        }

        return null;

    }
    private static async UniTask<PostTestRes> AuthPostTest(string msg, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new AuthPostTestReq(msg, PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<PostTestRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await AuthPostTest(msg, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }


        return null;

    }
    private static async UniTask<BasicProtocolRes> LogoutRequest(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoLogoutReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                PlayerClient.Reset();

                lock (pingLock) {

                    if (pingCts != null) {

                        pingCts.Cancel();
                        pingCts.Dispose();
                        pingCts = null;
                    
                    }

                }

                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await LogoutRequest(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {
            Debug.LogException(ex);
            Debug.Log(ex.ToString());
        }

        return null;
    
    }
    private static async UniTask<BasicProtocolRes> DeleteAccountRequest(string pw, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoAccountDeleteReq(PlayerClient.AuthToken, PlayerClient.UID, pw))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                PlayerClient.Reset();

                lock (pingLock) {

                    if (pingCts != null) {

                        pingCts.Cancel();
                        pingCts.Dispose();
                        pingCts = null;

                    }

                }

                return res;
            }

        } catch (TimeoutException ex) {


            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await DeleteAccountRequest(pw, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoAttendanceGetRes> GetAttendanceRequest(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoAttendanceGetReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoAttendanceGetRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetAttendanceRequest(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<BasicProtocolRes> SetAttendanceRequest(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoAttendanceSetReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await SetAttendanceRequest(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoAttendanceCheckRes> CheckAttendanceRequest(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoAttendanceCheckReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoAttendanceCheckRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await CheckAttendanceRequest(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoRewardRes> GetAttendanceReward(int day ,int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {
        
        try {

            var baseRes = await PostRequestAsync(new DtoAttendanceRewardReq(PlayerClient.AuthToken, PlayerClient.UID, day))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoRewardRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetAttendanceReward(day, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;
    
    }
    private static async UniTask<DtoMailListRes> GetMailList(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {
        
        try {

            var baseRes = await PostRequestAsync(new DtoMailListReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoMailListRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetMailList(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;
    
    }
    private static async UniTask<BasicProtocolRes> DeleteMail(int mail_id, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {
        
        try {

            var baseRes = await PostRequestAsync(new DtoMailDeleteReq(PlayerClient.AuthToken, PlayerClient.UID, mail_id))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await DeleteMail(mail_id, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;
    
    }
    private static async UniTask<DtoRewardRes> GetMailReward(int mail_id, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoMailRewardReq(PlayerClient.AuthToken, PlayerClient.UID, mail_id))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoRewardRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetMailReward(mail_id, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoUserItemInfosRes> GetDummy1List(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoUserDummy1InfosReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoUserItemInfosRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetDummy1List(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoUserItemInfosRes> GetDummy2List(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoUserDummy2InfosReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoUserItemInfosRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetDummy2List(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoUserItemInfosRes> GetDummy3List(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoUserDummy3InfosReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoUserItemInfosRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetDummy3List(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<BasicProtocolRes> SetAccountInfo(string nick_name, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoAccountInfoReq(PlayerClient.AuthToken, PlayerClient.UID, nick_name))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await SetAccountInfo(nick_name, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoFriendInfoListRes> GetFriendInfoList(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoFriendInfoListReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoFriendInfoListRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetFriendInfoList(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoFriendInfoListRes> GetFriendReqInfoList(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoFriendReqInfoListReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoFriendInfoListRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetFriendReqInfoList(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoFriendInfoListRes> GetFriendReceivedInfoList(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoFriendReceivedInfoListReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoFriendInfoListRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetFriendReceivedInfoList(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<BasicProtocolRes> FriendRequest(long friend_uid, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoFriendReqReq(PlayerClient.AuthToken, PlayerClient.UID, friend_uid))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await FriendRequest(friend_uid, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<BasicProtocolRes> FriendAccept(long friend_uid, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoFriendAcceptReq(PlayerClient.AuthToken, PlayerClient.UID, friend_uid))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await FriendAccept(friend_uid, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<BasicProtocolRes> FriendDelete(long friend_uid, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoFriendDeleteReq(PlayerClient.AuthToken, PlayerClient.UID, friend_uid))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await FriendDelete(friend_uid, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<BasicProtocolRes> FriendRequestCancel(long friend_uid, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoFriendReqCancelReq(PlayerClient.AuthToken, PlayerClient.UID, friend_uid))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await FriendRequestCancel(friend_uid, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<BasicProtocolRes> FriendRequestDeny(long friend_uid, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoFriendReqDenyReq(PlayerClient.AuthToken, PlayerClient.UID, friend_uid))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await FriendRequestDeny(friend_uid, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<BasicProtocolRes> SetScore(int score, int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoScoreSetReq(PlayerClient.AuthToken, PlayerClient.UID, score))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<BasicProtocolRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await SetScore(score, curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoGetMyRankingRes> GetMyRanking(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {
        
        try {

            var baseRes = await PostRequestAsync(new DtoGetMyRankingReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoGetMyRankingRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetMyRanking(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }
    private static async UniTask<DtoGetAllRankingRes> GetAllRankings(int curRetryCnt = 0, int maxRetryCnt = PlayerClient.RetryCnt) {

        try {

            var baseRes = await PostRequestAsync(new DtoGetAllRankingReq(PlayerClient.AuthToken, PlayerClient.UID))
                                .Timeout(TimeSpan.FromSeconds(PlayerClient.TimeOut), taskCancellationTokenSource: new CancellationTokenSource());

            if (TryParseResponse<DtoGetAllRankingRes>(baseRes, out var res)) {
                return res;
            }

        } catch (TimeoutException ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

            if (curRetryCnt >= maxRetryCnt) {

                Debug.LogError($"request fail - retry cnt over");
                return null;

            } else {

                Debug.Log($"retry - {curRetryCnt + 1}");
                return await GetAllRankings(curRetryCnt + 1, maxRetryCnt);

            }

        } catch (Exception ex) {

            Debug.LogException(ex);
            Debug.Log(ex.ToString());

        }

        return null;

    }

    #endregion

    #region client public method

    public static async UniTask<DtoHandShakeRes> HandShake() => await HandShake(0, PlayerClient.RetryCnt);
    public static async UniTask<DtoLoginRes> LoginRequest(string id, string pw) => await LoginRequest(id, pw, 0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> CreateAccountRequest(string id, string pw) => await CreateAccountRequest(id, pw, 0, PlayerClient.RetryCnt);
    public static async UniTask<PostTestRes> PostTest(string msg) => await PostTest(msg, 0, PlayerClient.RetryCnt);
    public static async UniTask<PostTestRes> AuthPostTest(string msg) => await AuthPostTest(msg, 0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> LogoutRequest() => await LogoutRequest(0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> DeleteAccountRequest(string pw) => await DeleteAccountRequest(pw, 0, PlayerClient.RetryCnt);
    public static async UniTask<DtoAttendanceGetRes> GetAttendanceRequest() => await GetAttendanceRequest(0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> SetAttendanceRequest() => await SetAttendanceRequest(0, PlayerClient.RetryCnt);
    public static async UniTask<DtoAttendanceCheckRes> CheckAttendanceRequest() => await CheckAttendanceRequest(0, PlayerClient.RetryCnt);
    public  static async UniTask<DtoRewardRes> GetAttendanceReward(int day) => await GetAttendanceReward(day, 0, PlayerClient.RetryCnt);
    public static async UniTask<DtoMailListRes> GetMailList() => await GetMailList(0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> DeleteMail(int mail_id) => await DeleteMail(mail_id, 0, PlayerClient.RetryCnt);
    public static async UniTask<DtoRewardRes> GetMailReward(int mail_id) => await GetMailReward(mail_id, 0, PlayerClient.RetryCnt);
    public static async UniTask<DtoUserItemInfosRes> GetDummy1List() => await GetDummy1List(0, PlayerClient.RetryCnt);
    public static async UniTask<DtoUserItemInfosRes> GetDummy2List() => await GetDummy2List(0, PlayerClient.RetryCnt);
    public static async UniTask<DtoUserItemInfosRes> GetDummy3List() => await GetDummy3List(0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> SetAccountInfo(string nick_name) => await SetAccountInfo(nick_name, 0, PlayerClient.RetryCnt);
    public static async UniTask<DtoFriendInfoListRes> GetFriendInfoList() => await GetFriendInfoList(0, PlayerClient.RetryCnt);
    public static async UniTask<DtoFriendInfoListRes> GetFriendReqInfoList() => await GetFriendReqInfoList(0, PlayerClient.RetryCnt);
    public static async UniTask<DtoFriendInfoListRes> GetFriendReceivedInfoList() => await GetFriendReceivedInfoList(0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> FriendRequest(long friend_uid) => await FriendRequest(friend_uid, 0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> FriendAccept(long friend_uid) => await FriendAccept(friend_uid, 0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> FriendDelete(long friend_uid) => await FriendDelete(friend_uid, 0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> FriendRequestCancel(long friend_uid) => await FriendRequestCancel(friend_uid, 0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> FriendRequestDeny(long friend_uid) => await FriendRequestDeny(friend_uid, 0, PlayerClient.RetryCnt);
    public static async UniTask<BasicProtocolRes> SetScore(int score) => await SetScore(score, 0, PlayerClient.RetryCnt);
    public static async UniTask<DtoGetMyRankingRes> GetMyRanking() => await GetMyRanking(0, PlayerClient.RetryCnt);
    public static async UniTask<DtoGetAllRankingRes> GetAllRankings() => await GetAllRankings(0, PlayerClient.RetryCnt);

    #endregion

}
