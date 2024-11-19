using ADHNetworkShared.Protocol.DTO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUnit : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI userName;
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI ranking;

    public void SetInfo(RankData data) {

        userName.text = data.nick_name;
        score.text = data.score.ToString();
        ranking.text = $"{data.ranking.ToString()}Rank";
    
    }

}
