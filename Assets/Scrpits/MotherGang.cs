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

    void Start()
    {
       
        gangMemberAnimators = new List<Animator>();

        //Create necessary objects
        inputX = new InputX();

        //CrateMembers
        memberToLoad = Resources.Load<Transform>("Prefabs/GangMember");

        if(DataManager.instance != null)
        {
            memberCount = DataManager.instance.levelData.memberCount;
            this.transform.position = DataManager.instance.levelData.motherGangPosition;

            Radius = SetRadius();

            CreateMembers();
        }

        SetGangMemberLists();
        
    }

    /*private void Update()
    {
        if (inputX.IsInput())
        {
            Move();
        }
    }*/

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
            if (isStationed)
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

        foreach (Animator animator in gangMemberAnimators)
        {
            animator.SetBool("isWalking", true);
        }
        

        Quaternion targetRotation = Quaternion.LookRotation(movementVec * 100f);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 35f);
    }
    void CreateMembers()
    {
        GangMemberTransforms = ObjectPooler.instance.PooltheObjects(memberToLoad, memberCount, this.transform,true);

        Members = new List<Member>(memberCount);

        foreach(Transform memberTransorm in GangMemberTransforms)
        {
            Vector3 memberPos = Radius * Random.insideUnitCircle;
            memberTransorm.transform.position = new Vector3(memberPos.x, memberToLoad.localScale.y / 2f, memberPos.y);
            Members.Add(memberTransorm.GetComponent<Member>());
        }

        //SetGangMemberLists();
    }


    //Decide circle radius which members will instantiate and move
    float SetRadius()
    {
        return memberToLoad.localScale.x / 2f * memberCount;
    }

    public void SetGangMemberLists()
    {
        gangMemberAnimators.Clear();

        gangMemberAnimators.AddRange(GetComponentsInChildren<Animator>());
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

