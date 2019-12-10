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

        public List<GangMember> MovableMembers;

        public List<GangMember> AllGang;
    }

    public struct GangMember
    {
        public GangMember(Transform memT, Member memM, Animator memA)
        {
            transform = memT;
            member = memM;
            animator = memA;
        }

        public Transform transform;
        public Animator animator;
        public Member member;
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
        DataManager.instance.currentGangState = GangState.Idle;

        memberCount = DataManager.instance.levelData.memberCount;
        this.transform.position = DataManager.instance.levelData.motherGangPosition;

        SetBase();

        CreateMembers();
    }

    private void Update()
    {
        
       if (inputX.IsInput() && (DataManager.instance.currentGangState != GangState.Climbing && DataManager.instance.currentGangState != GangState.Bridge))
       {
            GeneralInput gInput = inputX.GetInput(0);

            gangMovementScript.MoveTheGang(gInput, gang);
       }
    }

    void CreateMembers()
    {
        gang.MovableMembers = new List<GangMember>(memberCount);

        List<Transform> gangTransforms = ObjectPooler.instance.PooltheObjects(memberToLoad, memberCount, this.transform, true);

        for(int i = 0; i < memberCount; i++)
        {
            Transform memT = gangTransforms[i];
            Member memM = gangTransforms[i].GetComponent<Member>();
            Animator memA = gangTransforms[i].GetComponent<Animator>();

            Vector2 basePos = new Vector2(gang.Base.transform.position.x, gang.Base.transform.position.z);
            Vector2 memberPos = basePos + memM.SetPosInBase(gang.Base);

            memT.localPosition = new Vector3(memberPos.x, 0f, memberPos.y);

            gang.MovableMembers.Add(new GangMember(memT,memM,memA));
        }
        gang.AllGang = new List<GangMember>();
        gang.AllGang.AddRange(gang.MovableMembers);
    }

    //Gang in altinda yurucegi base i olustur
    void SetBase()
    {
        float radius = memberToLoad.localScale.x * memberCount / 2f;

        Transform gangBase = transform.GetChild(0);
        gangBase.localScale = new Vector3(radius, radius / 2f, radius);
        gangBase.localPosition = new Vector3(gangBase.transform.localPosition.x, gangBase.transform.localPosition.y + (gangBase.localScale.y), gangBase.transform.localPosition.z);

        GameObject baseHead = new GameObject("BaseHead");
        baseHead.transform.parent = gangBase;
        baseHead.transform.localScale = baseHead.transform.localScale * 0.1f;

        baseHead.transform.localPosition = new Vector3(0f, -1, 0.4f);

        gang.Base = gangBase;
    }


    private void OnTriggerEnter(Collider other)
    {

        //tag karsilastirmanin hizli youlu.Layer daha iyi olabilir
        if (other.CompareTag("LadderObstacle"))
        {
            DataManager.instance.currentGangState = GangState.Climbing;

            other.tag = "UsedObject";
            other.gameObject.layer = 9;

            int ladderManCount = other.GetComponent<Obstacle>().manCount;

            StartCoroutine(gangMovementScript.CreateLadder(gang, ladderManCount, other.transform));

           //StartCoroutine(gangMovementScript.CreateLadder(10, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), other.transform));   //gangMovementScript.gangTransforms[6]));                                  
        }

        if (other.CompareTag("BridgeObstacle"))
        {
            DataManager.instance.currentGangState = GangState.Bridge;

            other.tag = "UsedObject";
            other.gameObject.layer = 9;

            int bridgeManCount = other.GetComponent<Obstacle>().manCount;

            StartCoroutine(gangMovementScript.CreateBridge(gang, bridgeManCount, other.transform));

            //StartCoroutine(gangMovementScript.CreateBridge(8, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), other.transform));
        }

        if (other.CompareTag("FinishLine"))
        {
            //uIScript.LevelPassed();
            Debug.Log("Level Passed");
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

