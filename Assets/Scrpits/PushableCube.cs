using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableCube : Pushable
{
    void Update()
    {
        if (DataManager.instance.currentGangState == DataManager.GangState.Walking && isPushing())
            GetPushed();
    }



}
