using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestFriend : MonoBehaviour
{

    [SerializeField] private FriendUnit friendUnit;
    [SerializeField] private FriendReqUnit friendReqUnit;
    [SerializeField] private FriendRcvUnit friendRcvUnit;
    [SerializeField] private Transform parent;

    [SerializeField] private Button backBtn;
    [SerializeField] private Button friendBtn;
    [SerializeField] private Button friendReqBtn;
    [SerializeField] private Button friendRcvBtn;

    [SerializeField] private Button requestBtn;
    [SerializeField] private Button requestWindowBackBtn;
    [SerializeField] private Button sendBtn;
    [SerializeField] private GameObject requestWindow;
    [SerializeField] private TMP_InputField uidInputField;

    private List<FriendUnit> friendUnits = new List<FriendUnit>();
    private List<FriendReqUnit> friendReqUnits = new List<FriendReqUnit>();
    private List<FriendRcvUnit> friendRcvUnits = new List<FriendRcvUnit>();


    private void Start() {

        backBtn.onClick.AddListener(OnBackBtnClicked);
        friendBtn.onClick.AddListener(OnFriendBtnClicked);
        friendReqBtn.onClick.AddListener(OnFriendReqBtnClicked);
        friendRcvBtn.onClick.AddListener(OnFriendRcvBtnClicked);

        requestBtn.onClick.AddListener(OnRequestBtnClicked);
        requestWindowBackBtn.onClick.AddListener(OnRequestWindowBackBtnClicked);
        sendBtn.onClick.AddListener(OnSendBtnClicked);
    }

    private void OnEnable() {

        OnFriendBtnClicked();

    }

    private void OnRequestBtnClicked() {

        requestWindow.SetActive(true);

    }

    private void OnRequestWindowBackBtnClicked() {

        requestWindow.SetActive(false);

    }

    private void OnSendBtnClicked() {

        _ = OnSendBtnClickedTask();


    }

    private async UniTask OnSendBtnClickedTask() {
        var res = await ADHNetworkManager.FriendRequest(long.Parse(uidInputField.text));

        if (res != null) {
            Debug.Log(res.ToString());
        }
    }

    private void OnBackBtnClicked() {

        foreach (var unit in friendUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendUnits.Clear();

        foreach (var unit in friendReqUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendReqUnits.Clear();

        foreach (var unit in friendRcvUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendRcvUnits.Clear();

        gameObject.SetActive(false);

    }

    private void OnFriendBtnClicked() {

        _ = OnFriendBtnClickedTask();

    }

    private async UniTask OnFriendBtnClickedTask() {


        foreach (var unit in friendUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendUnits.Clear();

        foreach (var unit in friendReqUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendReqUnits.Clear();

        foreach (var unit in friendRcvUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendRcvUnits.Clear();

        var res = await ADHNetworkManager.GetFriendInfoList();

        if (res != null) {

            foreach (var item in res.userDatas) {

                var unit = Instantiate(friendUnit, parent);
                unit.SetInfo(item.nick_name, item.uid);
                friendUnits.Add(unit);

            }
        }

    }

    private void OnFriendReqBtnClicked() {

        _ = OnFriendReqBtnClickedTask();

    }

    private async UniTask OnFriendReqBtnClickedTask() {
        foreach (var unit in friendUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendUnits.Clear();

        foreach (var unit in friendReqUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendReqUnits.Clear();

        foreach (var unit in friendRcvUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendRcvUnits.Clear();

        var res = await ADHNetworkManager.GetFriendReqInfoList();

        if (res != null) {

            foreach (var item in res.userDatas) {

                var unit = Instantiate(friendReqUnit, parent);
                unit.SetInfo(item.nick_name, item.uid);
                friendReqUnits.Add(unit);

            }
        }
    }

    private void OnFriendRcvBtnClicked() {

        _ = OnFriendRcvBtnClickedTask();

    }

    private async UniTask OnFriendRcvBtnClickedTask() {

        foreach (var unit in friendUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendUnits.Clear();

        foreach (var unit in friendReqUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendReqUnits.Clear();

        foreach (var unit in friendRcvUnits) {
            if (unit != null) Destroy(unit.gameObject);
        }
        friendRcvUnits.Clear();

        var res = await ADHNetworkManager.GetFriendReceivedInfoList();

        if (res != null) {

            foreach (var item in res.userDatas) {

                var unit = Instantiate(friendRcvUnit, parent);
                unit.SetInfo(item.nick_name, item.uid);
                friendRcvUnits.Add(unit);

            }
        }

    }

}
