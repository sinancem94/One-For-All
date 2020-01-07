using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squasher : MonoBehaviour
{
    struct Arm
    {
        public Transform trans;
        public Vector3 IdlePosition;
    }

    List<Arm> Arms;

    Vector3 MiddlePos;

    public bool Squash;
    float MaxSpeed;
    void Start()
    {
        Arms = new List<Arm>();

        foreach (Transform trans in gameObject.transform)
        {
            Arm tmpArm = new Arm();
            tmpArm.trans = trans;
            tmpArm.IdlePosition = trans.position;

            Arms.Add(tmpArm);
        }

        CalculateMiddlePosition();

        MaxSpeed = 0.5f;
    }

    
    void Update()
    {
        if (Squash)
        {
            FastlySuqash();
        }
        else
        {
            SlowlyRetreat();
        }
    }

    void CalculateMiddlePosition()
    {
        Vector3 positionSum = Vector3.zero;
        foreach(Arm arm in Arms)
        {
            positionSum += arm.trans.position;
        }

        MiddlePos = positionSum / Arms.Count;
    }

    void FastlySuqash()
    {
        if (Vector3.Distance(Arms[0].trans.position, MiddlePos) < 0.2f || Vector3.Distance(Arms[1].trans.position, MiddlePos) < 0.2f)
        {
            Squash = false;
            return;
        }

        for (int i = 0; i < Arms.Count; i++)
        {
            float delta = (Vector3.Distance(Arms[i].IdlePosition, MiddlePos) * MaxSpeed) / Vector3.Distance(Arms[i].trans.position, MiddlePos);
            Arms[i].trans.position = Vector3.MoveTowards(Arms[i].trans.position, MiddlePos, delta);
        }
    }

    void SlowlyRetreat()
    {
        if (Vector3.Distance(Arms[0].trans.position, Arms[0].IdlePosition) < 0.1f && Vector3.Distance(Arms[1].trans.position, Arms[1].IdlePosition) < 0.1f)
        {
            Squash = true;
            return;
        }

        for(int i = 0; i < Arms.Count; i ++)
        {
            Arms[i].trans.position = Vector3.MoveTowards(Arms[i].trans.position, Arms[i].IdlePosition, 0.2f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("PushableObject"))
        {
            if(!Mathf.Approximately(other.transform.position.x, MiddlePos.x))
            {
                Vector3 goToPos = other.transform.position;
                goToPos.x = MiddlePos.x;

                other.transform.position = Vector3.MoveTowards(other.transform.position, goToPos, 1f);
            }
            else if(Squash)// eger ortaya yakinsa
            {
                Squash = false;
            }
            
        }

    }
}
