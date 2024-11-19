using ADHNetworkShared.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ADHNetworkShared.Protocol.DTO;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Test : MonoBehaviour
{

    [SerializeField] TMP_InputField accountID;
    [SerializeField] TMP_InputField accountPW;
    [SerializeField] TMP_InputField loginID;
    [SerializeField] TMP_InputField loginPW;

    public void TestHandshake() {
        _ = ADHNetworkManager.HandShake();
    }

    public void TestPost(string msg) {
         UniTask.RunOnThreadPool(async () => { 

            var res = await ADHNetworkManager.PostTest(msg); 
            
            Debug.Log(res.responseMSG);
        
        
        });
    }

    public void TestAuthPost(string msg) {
        UniTask.RunOnThreadPool(async () => {

            var res = await ADHNetworkManager.AuthPostTest(msg);

            Debug.Log(res.responseMSG);


        });
    }

    public void TestManyAuthPost(string msg) {
        UniTask.RunOnThreadPool(async () => {

            // 참고로 이렇게 하면 메인 쓰레드가 아니라서 유니티 꺼도 계속 실행된다.
            while (true) {

                var res = await ADHNetworkManager.AuthPostTest(msg);

                Debug.Log(res.responseMSG);

                await UniTask.Delay(10);
            }


        });
    }


    public void TestGetAttendance() {
        UniTask.RunOnThreadPool(async () => {

            var res = await ADHNetworkManager.GetAttendanceRequest();
            
            Debug.Log($"유저 아이디 - {res.uid}\n출석 횟수 - {res.attendance_cnt}\n최근 출석 날짜{res.recent_attendance_dt}");


        });
    }

    public void TestSetAttendance() {
        UniTask.RunOnThreadPool(async () => {

            var res = await ADHNetworkManager.SetAttendanceRequest();

            Debug.Log("ErrorCode :" + res.Result);


        });
    }


    public void TestCreateAccount() {

        UniTask.RunOnThreadPool(async () => {

            var res = await ADHNetworkManager.CreateAccountRequest(accountID.text, accountPW.text);

            Debug.Log("ErrorCode :" + res.Result);


        });
    }

    public void TestDeleteAccount(string pw) {

        UniTask.RunOnThreadPool(async () => {

            var res = await ADHNetworkManager.DeleteAccountRequest(pw);
            
            Debug.Log("ErrorCode :" + res.Result);


        });

    }

    public void TestLogin() {

        UniTask.RunOnThreadPool(async () => {

            var res = await ADHNetworkManager.LoginRequest(loginID.text, loginPW.text);

            Debug.Log("UserID :" + res);

        });

    }

    public void TestLogout() {

        UniTask.RunOnThreadPool(async () => {

            var res = await ADHNetworkManager.LogoutRequest();

            Debug.Log("ErrorCode :" + res.Result);

        });

    }

    public void TestCheckAttendance() {

        UniTask.RunOnThreadPool(async () => {

            var res = await ADHNetworkManager.CheckAttendanceRequest();
            
            Debug.Log("ErrorCode :" + res.Result);

            foreach (var attendance in res.rewards) { 
                
                Debug.Log($"receivable day: {attendance.Item1}");

                foreach (var item in attendance.Item2) {

                    Debug.Log("=======================================");
                    Debug.Log($"Type : {item.Item1.name}");
                    Debug.Log($"count : {item.Item2}");
                    Debug.Log("=======================================");
                
                }

                TestGetAttendanceReward(attendance.Item1);

            }

        });

    }


    public void TestGetAttendanceReward(int day) {

        UniTask.RunOnThreadPool(async () => {

            var res = await ADHNetworkManager.GetAttendanceReward(day);

            Debug.Log("ErrorCode :" + res.Result);

            

        });

    }

    public void TestGetDummy1List() {

        UniTask.RunOnThreadPool(async () => {

            while (true) {

                var res = await ADHNetworkManager.GetDummy1List();

                Debug.Log("ErrorCode :" + res.Result);

                foreach (var re in res.userItemInfos) {

                    Debug.Log("=======================================");
                    Debug.Log($"Item Name : {re.Item1.name}");
                    Debug.Log($"Count : {re.Item2}");
                    Debug.Log("=======================================");

                }

                await UniTask.Delay(10);

            }

        });

    }

    public void TestGetDummy2List() {

        UniTask.RunOnThreadPool(async () => {

            while (true) {

                var res = await ADHNetworkManager.GetDummy2List();

                Debug.Log("ErrorCode :" + res.Result);

                foreach (var re in res.userItemInfos) {

                    Debug.Log("=======================================");
                    Debug.Log($"Item Name : {re.Item1.name}");
                    Debug.Log($"Count : {re.Item2}");
                    Debug.Log("=======================================");

                }

                await UniTask.Delay(10);

            }

        });

    }

    public void TestGetDummy3List() {

        UniTask.RunOnThreadPool(async () => {

            while (true) {

                var res = await ADHNetworkManager.GetDummy3List();

                Debug.Log("ErrorCode :" + res.Result);

                foreach (var re in res.userItemInfos) {

                    Debug.Log("=======================================");
                    Debug.Log($"Item Name : {re.Item1.name}");
                    Debug.Log($"Count : {re.Item2}");
                    Debug.Log("=======================================");

                }

                await UniTask.Delay(10);

            }

        });

    }

}
