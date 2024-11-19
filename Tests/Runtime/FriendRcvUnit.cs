using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendRcvUnit : MonoBehaviour
{
    public long UID { get; set; }
    public string Name { get; set; }

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI uidText;
    [SerializeField] private Button receiveBtn;
    [SerializeField] private Button denyBtn;

    public void SetInfo(string name, long uid) {

        Name = name;
        UID = uid;

        nameText.text = name;
        uidText.text = uid.ToString();

        receiveBtn.onClick.RemoveAllListeners();
        denyBtn.onClick.RemoveAllListeners();

        receiveBtn.onClick.AddListener(OnReceiveBtnClicked);
        denyBtn.onClick.AddListener(OnDenyBtnClicked);

    }

    private void OnReceiveBtnClicked() {

        _ = OnReceiveBtnClickedTask();


    }

    private async UniTask OnReceiveBtnClickedTask() {
        var res = await ADHNetworkManager.FriendAccept(UID);

        if (res != null) {
            Debug.Log(res.ToString());
            Destroy(gameObject);
        }
    }

    private void OnDenyBtnClicked() {

        _ = OnDenyBtnClickedTask();


    }

    private async UniTask OnDenyBtnClickedTask() {
        var res = await ADHNetworkManager.FriendRequestDeny(UID);

        if (res != null) {
            Debug.Log(res.ToString());
            Destroy(gameObject);
        }
    }

}
