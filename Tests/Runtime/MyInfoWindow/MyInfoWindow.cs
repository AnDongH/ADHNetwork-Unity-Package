using ADHNetworkShared.Protocol;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyInfoWindow : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private TextMeshProUGUI uid;
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private TextMeshProUGUI scoreAndRanking;


    private void Start() {
        backBtn.onClick.AddListener(OnBackBtnClicked);
    }

    private void OnEnable() {

        _ = GetMyInfo();

    }
    
    private void OnBackBtnClicked() {
        gameObject.SetActive(false);
    }

    private async UniTask GetMyInfo() {

        var res = await ADHNetworkManager.GetMyRanking();

        if (res != null && res.Result == ErrorCode.None) {

            uid.text = res.MyRanking.uid.ToString();
            userName.text = res.MyRanking.nick_name;
            scoreAndRanking.text = $"{res.MyRanking.score}Score / {res.MyRanking.ranking}Rank";

        }

    }

}
