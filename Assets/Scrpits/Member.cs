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


    private void OnTriggerEnter(Collider other)
    {
        //tag karsilastirmanin hizli youlu.Layer daha iyi olabilir
        if(other.CompareTag("Ground"))
        {
            Debug.Log("grounded");
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        
        /*if (other.CompareTag("LadderObstacle") && !DataScript.memberCollisionLock)
        {
            DataScript.memberCollisionLock = true;
            other.gameObject.tag = "UsedObject";
            //StartCoroutine(gangMovementScript.CreateLadder(10, (int)transform.lossyScale.y * 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform),other.transform));                                       //gangMovementScript.gangTransforms[6]));                                  
        }*/

      

        if(other.gameObject.tag == "WreckingBall")
        {
            Debug.Log("wreckk");
            OpenRagdollPhysics();
        }

        if(other.gameObject.tag == "FinishLine")
        {
            uIScript.LevelPassed();
            Debug.Log("Level Passed");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground" && DataScript.isGravityOpen)
        {
            Debug.Log("ungrounded");
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
