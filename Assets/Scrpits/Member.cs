using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member : MonoBehaviour
{
    
    void Start()
    {
        CloseRagdollPhysics();
    }

    void OpenRagdollPhysics()
    {

    }

    void CloseRagdollPhysics()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
                collider.isTrigger = true;
        }
    }
}
