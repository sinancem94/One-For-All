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
        if (inputX.GetInputs() && (DataManager.instance.currentGangState == DataManager.GangState.Walking || DataManager.instance.currentGangState == DataManager.GangState.Idle))
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
            if (other.CompareTag("LadderObstacle"))
            {
                DataManager.instance.currentGangState = DataManager.GangState.Climbing;

                Obstacle ladder = other.GetComponent<Obstacle>();

                moveMethods.CreateObstaclePass(this, ladder.ManCount, ladder, Vector2.up);

                Debug.Log("Creating ladder");
                //StartCoroutine(moveMethods.CreateObstaclePassCorountine(DataManager.instance.GetGang(), ladder.ManCount, ladder, Vector2.up));

                //StartCoroutine(gangMovementScript.CreateLadder(10, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), other.transform));   //gangMovementScript.gangTransforms[6]));                                  
            }
            else if (other.CompareTag("BridgeObstacle"))
            {
                DataManager.instance.currentGangState = DataManager.GangState.Bridge;

                Obstacle bridge = other.GetComponent<Obstacle>();

                Debug.Log("Creating bridge");
                moveMethods.CreateObstaclePass(this, bridge.ManCount, bridge, Vector2.up);

                // StartCoroutine(moveMethods.CreateObstaclePassCorountine(DataManager.instance.GetGang(), bridge.ManCount, bridge, Vector2.up));

                //StartCoroutine(gangMovementScript.CreateBridge(8, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), other.transform));
            }
            else if (other.CompareTag("MemberLadder"))
            {
                Obstacle createdLadder = other.GetComponentInParent<Obstacle>();

                if (createdLadder.isCloseToPassPoint(DataManager.instance.GetGang().baseHead.position))
                {
                    DataManager.instance.currentGangState = DataManager.GangState.Climbing;

                    Vector2 directionVec;

                    if (DataManager.instance.GetGang().Base.position.y < createdLadder.passStartPosition.y)
                        directionVec = Vector2.up;
                    else
                        directionVec = Vector2.down;

                    moveMethods.PassObstacle(this, createdLadder, directionVec);
                    Debug.Log("Passing ladder");
                    // StartCoroutine(moveMethods.PassObjectCorountine(DataManager.instance.GetGang(), createdLadder, directionVec));

                }
            }
            else if (other.CompareTag("MemberBridge"))
            {
                Obstacle createdBridge = other.GetComponentInParent<Obstacle>();

                if (createdBridge.isCloseToPassPoint(DataManager.instance.GetGang().baseHead.position))
                {
                    DataManager.instance.currentGangState = DataManager.GangState.Bridge;

                    Vector2 directionVec;

                    if (DataManager.instance.GetGang().Base.position.z < createdBridge.passStartPosition.z)
                        directionVec = Vector2.up;
                    else
                        directionVec = Vector2.down;

                    moveMethods.PassObstacle(this, createdBridge, directionVec);

                    Debug.Log("Passing bridge");
                }
            }
        }

       
        if (other.CompareTag("FinishLine"))
        {
            DataManager.instance.LevelPassed();
            Debug.Log("Level Passed");
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
