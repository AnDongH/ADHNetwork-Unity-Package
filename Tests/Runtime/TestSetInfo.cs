using ADHNetworkShared.Protocol;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestSetInfo : MonoBehaviour
{

    [SerializeField] private Button sendBtn;
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] protected TMP_InputField inputField;


    private void Start() {

        sendBtn.onClick.AddListener(OnBtnClicked);

    }

    private void OnBtnClicked() {
        
        _ = SetInfo();

    }

    private async UniTask SetInfo() {

        var res = await ADHNetworkManager.SetAccountInfo(inputField.text);

        if (res != null && res.Result == ErrorCode.None) {
            userName.text = inputField.text;
            Debug.Log("success change user nick name");
        }

    }

}
