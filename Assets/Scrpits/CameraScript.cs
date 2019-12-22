using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject objectFollowedByCam;

    void Update()
    {
        //Calculate the delta
        float delta = Mathf.Abs(Vector3.Distance(transform.localPosition, objectFollowedByCam.transform.position)) * 0.03f;

        transform.position = Vector3.MoveTowards(transform.position,objectFollowedByCam.transform.position,delta);    
    }
}
