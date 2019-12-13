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
        Bridge,
        LevelPassed,
        GameOver
    };

    public struct Gang
    {
        public GangState currState;
        //gangin ustunde gitcegi base
        public Transform Base;
        public Transform baseHead;

        //Dynamic List . When climbing, building ladder etc. this list will be cleared and remaning movableMembers will be added one by one
        public List<GangMember> MovableMembers;

        //Movable member larin tumu. Eger bir member olduyse veya kopru, merdiven vs olduysa burdan cikar
        public List<GangMember> AllGang;

        public Collision collision;
        public Collider collider;
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

    private void LateUpdate()
    {
        if (DataManager.instance.State == DataManager.GameState.Play && gang.AllGang.Count == 0)
        {
            DataManager.instance.GameOver();
        }
    }

    private void Update()
    {
       if (inputX.IsInput() && (DataManager.instance.currentGangState == GangState.Walking || DataManager.instance.currentGangState == GangState.Idle))
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
            Vector2 memberPos = basePos + memM.SetRandomPositionInBase(gang.Base);

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
        gang.baseHead = baseHead.transform;

        gang.collider = gang.Base.GetChild(0).GetComponent<Collider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        //tag karsilastirmanin hizli youlu.Layer daha iyi olabilir
        if (other.CompareTag("LadderObstacle"))
        {
            DataManager.instance.currentGangState = GangState.Climbing;

            Obstacle ladder = other.GetComponent<Obstacle>();

            StartCoroutine(gangMovementScript.CreateLadder(gang, ladder.ManCount, ladder,Vector2.up));

           //StartCoroutine(gangMovementScript.CreateLadder(10, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), other.transform));   //gangMovementScript.gangTransforms[6]));                                  
        }
        else if (other.CompareTag("BridgeObstacle"))
        {
            DataManager.instance.currentGangState = GangState.Bridge;

            Obstacle bridge = other.GetComponent<Obstacle>();

            StartCoroutine(gangMovementScript.CreateBridge(gang, bridge.ManCount, bridge,Vector2.up));

            //StartCoroutine(gangMovementScript.CreateBridge(8, 3, gangMovementScript.gangTransforms.Find(x => x.transform == transform), other.transform));
        }
        else if(other.CompareTag("MemberLadder"))
        {
            Obstacle createdLadder = other.GetComponent<Obstacle>();

            if(createdLadder.isCloseToPassPoint(gang.baseHead.position))
            {
                DataManager.instance.currentGangState = GangState.Climbing;

                Vector2 directionVec;

                if (gang.Base.position.y < createdLadder.transform.position.y)
                    directionVec = Vector2.up;
                else
                    directionVec = Vector2.down;

                StartCoroutine(gangMovementScript.PassLadder(gang, createdLadder, directionVec));

            }
        }
        else if (other.CompareTag("MemberBridge"))
        {
            Obstacle createdBridge = other.GetComponent<Obstacle>();

            if (createdBridge.isCloseToPassPoint(gang.baseHead.position))
            {
                DataManager.instance.currentGangState = GangState.Climbing;

                Vector2 directionVec;

                if (gang.Base.position.z < createdBridge.transform.position.z)
                    directionVec = Vector2.up;
                else
                    directionVec = Vector2.down;

                StartCoroutine(gangMovementScript.PassBridge(gang, createdBridge, directionVec));

            }
        }
        else if (other.CompareTag("FinishLine"))
        {
            DataManager.instance.LevelPassed();
            Debug.Log("Level Passed");
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("HouseObstacle"))
        {
            //Debug.Log("anan");
            gang.collision = collision;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("HouseObstacle"))
        {
            gang.collision = null;
        }
    }

    public Gang GetGang()
    {
        return gang;
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

