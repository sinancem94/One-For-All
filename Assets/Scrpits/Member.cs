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
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
            {
                collider.isTrigger = true;
            }
        }

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            if(rigidbody.gameObject != this.gameObject)
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LadderObstacle" && !DataScript.memberCollisionLock)
        {
            DataScript.memberCollisionLock = true;
            collision.gameObject.tag = "UsedObject";
            collision.gameObject.layer = 9;
            StartCoroutine(gangMovementScript.CreateLadder(5, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), collision.gameObject.transform));                                       //gangMovementScript.gangTransforms[6]));                                  
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ground")
        {
            Debug.Log("grounded");
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        
        if (other.gameObject.tag == "LadderObstacle" && !DataScript.memberCollisionLock)
        {
            DataScript.memberCollisionLock = true;
            other.gameObject.tag = "UsedObject";
            StartCoroutine(gangMovementScript.CreateLadder(5, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform),other.transform));                                       //gangMovementScript.gangTransforms[6]));                                  
        }
    }
}
