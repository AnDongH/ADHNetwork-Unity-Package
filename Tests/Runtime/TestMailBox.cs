using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMailBox : MonoBehaviour
{

    [SerializeField] private Button backButton;
    [SerializeField] private Transform btnParent;
    [SerializeField] private TestMailUnit mailUnitPrefab;
    [SerializeField] private TestMailWindow window;

    private List<TestMailUnit> testMailUnits = new List<TestMailUnit>();

    private void Start() {
        backButton.onClick.AddListener(Back);
    }


    private void Back() {

        foreach (var mailUnit in testMailUnits) {
            if (mailUnit != null) Destroy(mailUnit.gameObject);
        }
        testMailUnits.Clear();

        gameObject.SetActive(false);
    }

    private void OnEnable() {

        _ = OpenMailBox();

    }

    private async UniTask OpenMailBox() {

        foreach (var mailUnit in testMailUnits) Destroy(mailUnit.gameObject);
        testMailUnits.Clear();

        var res = await ADHNetworkManager.GetMailList();

        if (res == null) return;

        for (int i = 0; i < res.mailInfos.Count; i++) {

            TestMailUnit u = Instantiate(mailUnitPrefab, btnParent);
            u.SetInfo(res.mailInfos[i], window);
            testMailUnits.Add(u);

        }

    }

}
