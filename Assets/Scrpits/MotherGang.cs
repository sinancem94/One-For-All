using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;

public class MotherGang : MonoBehaviour
{
    Transform memberToLoad;
    List<Transform> GangMemberTransforms;
    public List<Member> Members;
    List<Animator> gangMemberAnimators;

    int memberCount;
    
    InputX inputX;

    private GangMovementScript gangMovementScript;
    float Radius; //adamlarin olusabilecegi, ilerleyebilcegi daire buyuklugu
    GangState currGangState;

    public enum GangState
    {
        Idle = 0,
        Walking,
        Climbing,
        Bridge
    };

    void Start()
    {
        //Create necessary objects
        inputX = new InputX();

        //CrateMembers
        memberToLoad = Resources.Load<Transform>("Prefabs/GangMember");

        currGangState = GangState.Idle;

        if(DataManager.instance != null)
        {
            memberCount = DataManager.instance.levelData.memberCount;
            this.transform.position = DataManager.instance.levelData.motherGangPosition;

            Radius = SetRadius();

            CreateMembers();
        }
        
    }
    
    private void Update()
   {
       if (inputX.IsInput() && (currGangState != GangState.Climbing || currGangState != GangState.Bridge))
       {
            if (currGangState != GangState.Walking)
                currGangState = GangState.Walking;

            Move();
       }
   }

    Vector2 touchStartPos;
    Vector2 touchDelta;

    public void Move()
    {
        GeneralInput gInput = inputX.GetInput(0);

        if (gInput.phase == IPhase.Began)
        {
            touchStartPos = gInput.currentPosition;
        }
        else if (gInput.phase == IPhase.Ended)
        {
            touchDelta = Vector2.zero;
        }
        else if (gInput.phase == IPhase.Moved  || gInput.phase == IPhase.Stationary)
        {
            touchDelta = (gInput.currentPosition - touchStartPos);
            Debug.Log(Vector2.Distance(touchStartPos, gInput.currentPosition));
            touchStartPos = Vector2.MoveTowards(touchStartPos, gInput.currentPosition,Vector2.Distance(touchStartPos, gInput.currentPosition) / 50f);

            JoyStickMovement(touchDelta);
        }
    }

    void JoyStickMovement(Vector2 posDelta)
    {
        Vector3 posVec = transform.position;
        posVec.x += posDelta.x;
        posVec.z += posDelta.y;
        transform.position = Vector3.MoveTowards(transform.position, posVec, 0.8f);

        Quaternion targetRotation = Quaternion.LookRotation(posVec * 100f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 35f);
    }


    void CreateMembers()
    {
        GangMemberTransforms = ObjectPooler.instance.PooltheObjects(memberToLoad, memberCount, this.transform,true);

        Members = new List<Member>(memberCount);
        gangMemberAnimators = new List<Animator>(memberCount);

        foreach (Transform memberTransorm in GangMemberTransforms)
        {
            Vector3 memberPos = Radius * Random.insideUnitCircle;
            memberTransorm.transform.position = new Vector3(memberPos.x, memberToLoad.localScale.y / 2f, memberPos.y);

            Members.Add(memberTransorm.GetComponent<Member>());
            gangMemberAnimators.Add(memberTransorm.GetComponent<Animator>());
        }
    }


    //Decide circle radius which members will instantiate and move
    float SetRadius()
    {
        return memberToLoad.localScale.x / 5f * memberCount;
    }

}
























/*
    void Start()
    {
        //CrateMembers
        memberToLoad = Resources.Load<Transform>("Prefabs/GangMember");
        memberCount = DataManager.instance.levelData.memberCount;
        this.transform.position = DataManager.instance.levelData.motherGangPosition;

        Radius = SetRadius();

        CreateMembers();

        

        gangMemberAnimators = new List<Animator>();

        //Create necessary objects
        inputX = new InputX();
    }

    private void Update()
    {
        if (inputX.IsInput())
        {
            Move();
        }
    }

    Vector2 touchStartPos;
    Vector2 touchDelta;

    bool isStationed = false;
    Vector2 stationPos; 


    public void Move()
    {
        GeneralInput gInput = inputX.GetInput(0);

        if (gInput.phase == IPhase.Began)
        {
            touchStartPos = gInput.currentPosition;

            stationPos = touchStartPos;
            isStationed = false;
        }
        else if (gInput.phase == IPhase.Ended)
        {   
            touchDelta = Vector2.zero;
        }
        else if (gInput.phase == IPhase.Moved)
        {
            if(isStationed)
            {
                isStationed = false;
                touchStartPos = stationPos; 
            }

            touchDelta = (gInput.currentPosition - touchStartPos) / (Screen.width / 2f);
            
            touchStartPos += 0.1f * touchDelta;

            JoyStickMovement(touchDelta);
        }
        else if (gInput.phase == IPhase.Stationary)
        {
            if (!isStationed)
                isStationed = true;

            stationPos = Vector2.MoveTowards(stationPos, gInput.currentPosition, Screen.width / 100); //(gInput.currentPosition - touchStartPos) / (Screen.width / 2f);
            Debug.Log(touchStartPos + " delta " + touchDelta);
            touchDelta = (gInput.currentPosition - stationPos) / (Screen.width / 2f);

            JoyStickMovement(touchDelta);
        }
    }

    void JoyStickMovement(Vector2 posDelta)
    {
        Vector2 delta = Vector2.ClampMagnitude(touchDelta, 10);
        Vector3 movementVec = new Vector3(delta.x, 0f, delta.y);

        transform.position += movementVec;

        foreach(Animator animator in gangMemberAnimators)
        {
            animator.SetBool("isWalking", true);
        }

        Quaternion targetRotation = Quaternion.LookRotation(movementVec * 100f);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 35f);
    }*/

