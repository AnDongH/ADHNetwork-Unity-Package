using ADHNetworkShared.Protocol;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingWindow : MonoBehaviour
{

    [SerializeField] private Button backBtn;
    [SerializeField] private ScoreUnit scoreUnitPrefab;
    [SerializeField] private Transform parent;

    private List<ScoreUnit> units = new List<ScoreUnit>();

    private void Start() {

        backBtn.onClick.AddListener(OnBackBtnClicked);

    }

    private void OnEnable() {

        _ = GetAllRankings();

    }

    private void OnBackBtnClicked() {

        foreach (var unit in units) {
            if (unit != null) Destroy(unit.gameObject);
        }
        units.Clear();

        gameObject.SetActive(false);

    }

    private async UniTask GetAllRankings() {

        foreach (var unit in units) {
            if (unit != null) Destroy(unit.gameObject);
        }
        units.Clear();

        var res = await ADHNetworkManager.GetAllRankings();

        if (res != null && res.Result == ErrorCode.None) {

            foreach (var ranking in res.AllRankings) {

                ScoreUnit unit = Instantiate(scoreUnitPrefab, parent);
                unit.SetInfo(ranking);
                units.Add(unit);

            }

        }

    }

}
