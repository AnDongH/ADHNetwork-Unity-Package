using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScoreBtn : MonoBehaviour
{
    
    public void SetScore(int score) {

        _ = SetScoreTask(score);
    
    }

    private async UniTask SetScoreTask(int score) {

        var res = await ADHNetworkManager.SetScore(score);

        Debug.Log(res.ToString());

    }

}
