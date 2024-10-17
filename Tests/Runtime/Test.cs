using ADHNetworkShared.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MemoryPack;

public class Test : MonoBehaviour
{
    public void TestHandshake() {
        _ = ADHNetworkManager.HandShake();
    }

    public void TestPost() {
        _ = ADHNetworkManager.PostRequestAsync(new PostTestReq("/post", "adh"));
    }
}
