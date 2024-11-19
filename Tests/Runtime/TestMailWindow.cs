using ADHNetworkShared.Protocol;
using ADHNetworkShared.Protocol.DTO;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestMailWindow : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private TestRewardUnit rewardUnitPrefab;
    [SerializeField] private Transform rewardParent;
    [SerializeField] private Button receiveBtn;

    private MailInfo mailInfo;
    private List<TestRewardUnit> testRewardUnits = new List<TestRewardUnit>();

    public void SetInfo(MailInfo mailInfo) {

        foreach (var item in testRewardUnits) Destroy(item.gameObject);
        testRewardUnits.Clear();

        title.text = mailInfo.title;
        content.text = mailInfo.content;
        this.mailInfo = mailInfo;

        if (mailInfo.rewards.Count != 0) {

            receiveBtn.gameObject.SetActive(true);
            receiveBtn.interactable = !mailInfo.is_received;
            receiveBtn.onClick.RemoveAllListeners();
            receiveBtn.onClick.AddListener(() => OnReceiveBtnClicked(receiveBtn));

            for (int i = 0; i < mailInfo.rewards.Count; i++) {

                TestRewardUnit u = Instantiate(rewardUnitPrefab, rewardParent);
                u.SetInfo(mailInfo.rewards[i].Item1.name, mailInfo.rewards[i].Item2.ToString());
                testRewardUnits.Add(u);

            }

        
        } else {
            
            receiveBtn.gameObject.SetActive(false);
        
        }

    }

    private void OnReceiveBtnClicked(Button button) {
        _ = GetReward(button);
    }

    private async UniTask GetReward(Button button) {

        var res = await ADHNetworkManager.GetMailReward(mailInfo.mail_id);

        Debug.Log("ErrorCode :" + res.Result);

        if (res.Result == ErrorCode.None) {
            button.interactable = false;
            mailInfo.is_received = true;
        }

    }

}
