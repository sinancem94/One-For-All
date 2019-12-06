using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member : MonoBehaviour
{
    private GangMovementScript gangMovementScript;
    private UIScript uIScript;
    
    void Start()
    {
        gangMovementScript = FindObjectOfType(typeof(GangMovementScript)) as GangMovementScript;
        uIScript = FindObjectOfType(typeof(UIScript)) as UIScript;
        CloseRagdollPhysics();
    }

    void OpenRagdollPhysics()
    {
        transform.parent = null;
        gangMovementScript.SetGangList();
        Collider[] colliders = GetComponentsInChildren<Collider>();
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Collider collider in colliders)
        {
                collider.isTrigger = false;
        }

        foreach (Rigidbody rigidbody in rigidbodies)
        {
                rigidbody.useGravity = true;
                //rigidbody.isKinematic = false;
        }
        GetComponent<Animator>().enabled = false;
        
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
                //rigidbody.isKinematic = true;
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
            StartCoroutine(gangMovementScript.CreateLadder(10, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), collision.transform));                                       //gangMovementScript.gangTransforms[6]));                                  
        }

        if (collision.gameObject.tag == "BridgeObstacle" && !DataScript.memberCollisionLock)
        {
            DataScript.memberCollisionLock = true;
            collision.gameObject.tag = "UsedObject";
            collision.gameObject.layer = 9;
            StartCoroutine(gangMovementScript.CreateBridge(8, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), collision.transform));
        }

        if (collision.gameObject.tag == "WreckingBall")
        {
            Debug.Log("wreckk");
            OpenRagdollPhysics();
        }

        if (collision.gameObject.tag == "FinishLine")
        {
            uIScript.LevelPassed();
            Debug.Log("Level Passed");
        }
    }

}
