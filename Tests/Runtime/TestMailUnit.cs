using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestMailUnit : MonoBehaviour
{

    [SerializeField] private Button unitBtn;
    [SerializeField] private Button removeBtn;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI remain;


    private MailInfo mailInfo;
    private TestMailWindow window;

    private void Start() {
        unitBtn.onClick.AddListener(OpenMail);
    }

    private void OpenMail() {

        window.gameObject.SetActive(true);
        window.SetInfo(mailInfo);

    }

    public void SetInfo(MailInfo mailInfo, TestMailWindow window) {
        this.mailInfo = mailInfo;
        this.window = window;
        title.text = mailInfo.title;
        remain.text = $"remain {mailInfo.remain_time}days";
    }

    public void Remove() {
        _ = RemoveReq();
    }

    private async UniTask RemoveReq() {

        var res = await ADHNetworkManager.DeleteMail(mailInfo.mail_id);

        Debug.Log("ErrorCode :" + res.Result);

        if (res.Result == ErrorCode.None) Destroy(gameObject);

    }

}
