using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    Transform pushingObject;

    void Start()
    {
        string tagStr = "PushableObject";

        gameObject.tag = tagStr;
        gameObject.layer = LayerMask.NameToLayer(tagStr);
    }

    public void GetPushed()
    {       
        this.transform.position += transform.forward * DataManager.instance.GetGang().speed / 2f;

        transform.rotation = pushingObject.rotation;
    }

    public bool isPushing()
    {
        if (pushingObject != null)
            return true;
        return false;
    }

    public void CollidedWithPusher(GameObject pusher)
    {
        if (pushingObject == null)
        {
            pushingObject = pusher.transform;
        }
    }

    public void ExitedCollisionWithPusher()
    {
        if (pushingObject != null)
            pushingObject = null; 
    }
}
