using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;

public class MotherGang : MonoBehaviour
{
    int memberCount;
    Transform memberToLoad;

    public enum GangState
    {
        Idle = 0,
        Walking,
        Climbing,
        Bridge
    };

    public struct Gang
    {
        public GangState currState;

        //gangin ustunde gitcegi base
        public Transform Base;

        public List<Transform> Transforms;
        public List<Member> Members;
        public List<Animator> Animators;
    }

    Gang gang;
    InputX inputX;

    private GangMovementScript gangMovementScript;

    void Start()
    {
        //Create necessary objects
        inputX = new InputX();
        gangMovementScript = GetComponent<GangMovementScript>();

        //CrateMembers
        memberToLoad = Resources.Load<Transform>("Prefabs/GangMember");

        gang = new Gang();

        gang.currState = GangState.Idle;

        memberCount = DataManager.instance.levelData.memberCount;
        this.transform.position = DataManager.instance.levelData.motherGangPosition;

        SetBase();

        CreateMembers();
    }

    private void Update()
    {
       if (inputX.IsInput() && (gang.currState != GangState.Climbing || gang.currState != GangState.Bridge))
       {
            if (gang.currState != GangState.Walking)
                gang.currState = GangState.Walking;

            GeneralInput gInput = inputX.GetInput(0);

            gangMovementScript.MoveTheGang(gInput, gang);
       }
    }

    void CreateMembers()
    {
        gang.Transforms = ObjectPooler.instance.PooltheObjects(memberToLoad, memberCount, this.transform, true);

        gang.Members = new List<Member>(memberCount);
        gang.Animators = new List<Animator>(memberCount);

        foreach (Transform memberTransorm in gang.Transforms)
        {
            Vector3 memberPos = gang.Base.localScale.x / 2f * Random.insideUnitCircle;
            memberTransorm.transform.position = new Vector3(memberPos.x, memberToLoad.localScale.y / 2f, memberPos.y);

            gang.Members.Add(memberTransorm.GetComponent<Member>());
            gang.Animators.Add(memberTransorm.GetComponent<Animator>());
        }
    }

    //Gang in altinda yurucegi base i olustur
    void SetBase()
    {
        float radius = memberToLoad.localScale.x * memberCount;

        Transform gangBase = transform.GetChild(0);
        gangBase.localScale = new Vector3(radius, radius, radius);

        gang.Base = gangBase;
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LadderObstacle"))
        {
            DataScript.memberCollisionLock = true;
            other.gameObject.tag = "UsedObject";
            StartCoroutine(gangMovementScript.CreateLadder(10, (int)transform.lossyScale.y * 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), other.transform));                                       //gangMovementScript.gangTransforms[6]));                                  
        }

        if (other.CompareTag("BridgeObstacle") && !DataScript.memberCollisionLock)
        {
            DataScript.memberCollisionLock = true;
            other.gameObject.tag = "UsedObject";
            StartCoroutine(gangMovementScript.CreateBridge(8, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), other.transform));
        }
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

