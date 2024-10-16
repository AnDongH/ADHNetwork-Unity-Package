using ADHNetworkShared.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostTestHandler : IProtocolHandler {
    public void Process(ProtocolRes response) {

        PostTestRes res = response as PostTestRes;
        Debug.Log(res.content);

    }
}
