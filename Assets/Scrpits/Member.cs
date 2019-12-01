using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member : MonoBehaviour
{
    private GangMovementScript gangMovementScript;
    
    void Start()
    {
        gangMovementScript = FindObjectOfType(typeof(GangMovementScript)) as GangMovementScript;
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

    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Oncollisionenter");
        if (other.gameObject.tag == "LadderObstacle" && !DataScript.memberCollisionLock)
        {
            DataScript.memberCollisionLock = true;
            StartCoroutine(gangMovementScript.CreateLadder(5, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform),other.transform));                                       //gangMovementScript.gangTransforms[6]));                                  
        }
    }
}
