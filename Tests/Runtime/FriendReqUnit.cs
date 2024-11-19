using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendReqUnit : MonoBehaviour
{
    public long UID { get; set; }
    public string Name { get; set; }

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI uidText;
    [SerializeField] private Button cancelBtn;



    public void SetInfo(string name, long uid) {

        Name = name;
        UID = uid;

        nameText.text = name;
        uidText.text = uid.ToString();

        cancelBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.AddListener(OnBtnClicked);

    }

    private void OnBtnClicked() {

        _ = OnBtnClickedTask();


    }

    private async UniTask OnBtnClickedTask() {
        var res = await ADHNetworkManager.FriendRequestCancel(UID);

        if (res != null) {
            Debug.Log(res.ToString());
            Destroy(gameObject);
        }
    }

}
