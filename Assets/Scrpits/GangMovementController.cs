using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;

public class GangMovementController : MonoBehaviour
{
    private GangMovementMethods moveMethods;
    InputX inputX;

    GeneralInput gInput;

    void Start()
    {
        inputX = new InputX();
        moveMethods = new GangMovementMethods();
    }

    private void Update()
    {
        if (inputX.GetInputs() && (DataManager.instance.currentGangState != DataManager.GangState.EventLocked))
        {
            inputDelta = SetInputStateAndDelta();
        }
    }

    private void FixedUpdate()
    {
        if (DataManager.instance.currentGangState == DataManager.GangState.Walking)
        {
            moveMethods.JoyStickMovement(DataManager.instance.GetGang(), inputDelta);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //eger herhangi bir event yoksa su an
        if(moveMethods.gangEvent == null)
        {
            //tag karsilastirmanin hizli youlu.Layer daha iyi olabilir
            if (other.CompareTag("Obstacle"))
            {
                DataManager.instance.currentGangState = DataManager.GangState.EventLocked;

                Obstacle obstacle = other.GetComponent<Obstacle>();

                moveMethods.CreateObstaclePass(this, obstacle.ManCount, obstacle);

                Debug.Log("Creating ladder or bridge");                                
            }
            else if (other.CompareTag("PassableObstacle"))
            {
                Obstacle createdPass = other.GetComponentInParent<Obstacle>();

                if (createdPass.isCloseToPassPoint(DataManager.instance.GetGang().baseHead.position))
                {
                    DataManager.instance.currentGangState = DataManager.GangState.EventLocked;
                   
                    moveMethods.PassObstacle(this, createdPass);
                    Debug.Log("Passing ladder or bridge");
                }
            }
          
        }
 
        if (other.CompareTag("FinishLine"))
        {
            DataManager.instance.LevelPassed();
            Debug.Log("Level Passed");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("PushableObject"))
        {
            collision.gameObject.GetComponent<Pushable>().CollidedWithPusher(this.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("PushableObject"))
        {
            collision.gameObject.GetComponent<Pushable>().ExitedCollisionWithPusher();
        }
    }


        Vector2 inputStartPos;
    Vector2 inputDelta = Vector2.zero;
    public Vector2 SetInputStateAndDelta()
    {
        gInput = inputX.GetInput(0);

        if (gInput.phase == IPhase.Began)
        {
            inputStartPos = gInput.currentPosition;

            inputDelta = Vector2.zero;

            DataManager.instance.currentGangState = DataManager.GangState.Walking;

        }
        else if (gInput.phase == IPhase.Ended)
        {
            inputDelta = Vector2.zero;
            
            DataManager.instance.currentGangState = DataManager.GangState.Idle;
        }
        else 
        {
            inputDelta = (gInput.currentPosition - inputStartPos);

            //move starting position towards to current place
            inputStartPos = Vector2.MoveTowards(inputStartPos, gInput.currentPosition, Vector2.Distance(inputStartPos, gInput.currentPosition) / 50f);
        }

        return inputDelta;
    }

}
