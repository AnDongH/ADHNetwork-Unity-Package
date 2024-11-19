using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestRewardUnit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemCnt;

    public void SetInfo(string name, string cnt) {
        itemName.text = name;
        itemCnt.text = $"X {cnt}";
    }
}
