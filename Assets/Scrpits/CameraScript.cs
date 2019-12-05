using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject objectFollowedByCam;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,objectFollowedByCam.transform.position,1f);    //bunu daha düzgün implement et
    }
}
