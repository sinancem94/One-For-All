using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Member : MonoBehaviour
{
    MemberActions actions;

    public Rigidbody memRb;
    public Animator memAnim;

    //member larin hareketini saglamak icin mutex. eger tirmanma veya kopru gecme vs islemindeyse lock la yoksa acik dursun
    bool memberLock;

    //ilk basta MotherGang de setleniyor
    Vector3 positionInBase;
    Vector2 RandomPosInBase;

    //basePos her framede updateleniyor eger farkli ise positionInBase degistiriliyor
    Vector2 basePosition;

    //for walking animation. if memberLock is UNLOCKED Member will start walk animation automaticaly if position changed. 
    Vector3 currPosition;
    bool isWalking = false;

    void Start()
    {
        memRb = GetComponent<Rigidbody>();
        memAnim = GetComponent<Animator>();

        SetLock(false);
        actions = new MemberActions();
        basePosition = Vector2.one * 10f;

        CloseRagdollPhysics();
    }

    private void Update()
    {
        positionInBase = SetNewPosition(DataManager.instance.GetGang().Base);
    }

    private void FixedUpdate()
    {
        if (!memberLock) //&& (DataManager.instance.currentGangState == DataManager.GangState.Walking || DataManager.instance.currentGangState == DataManager.GangState.Idle))
        {
            MoveTowards(positionInBase);
            WalkAnimation();
        }
    }

    void OpenRagdollPhysics()
    {
        transform.parent = null;

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
            if (rigidbody.gameObject != this.gameObject)
            {
                rigidbody.useGravity = false;
                //rigidbody.isKinematic = true;
            }
        }
    }

    public void CreateObstaclePass(MotherGang.Gang gang, MotherGang.GangMember member, Obstacle obstacle, Vector3 passStartPos, Vector3 memberPosInPass, Action SetBasePosition = null)
    {
        //MotherGang.GangMember member = gang.AllGang.Find(mem => mem.member == this);
        //Eger member movable ise cikar ordan
        RemoveMemberFromMovables(gang);

        //if bridge set as -1 . member will look downwards
        //if ladder set as 0. will not call LookAt for member 
        //if otherthere is a error set as 0
        int lookDirection = (obstacle.ObstacleType == Obstacle.Type.Bridge) ? -1 : (obstacle.ObstacleType == Obstacle.Type.Ladder) ? 0 : 0;

        StartCoroutine(actions.BePassPart(member, passStartPos, memberPosInPass, SetBasePosition, lookDirection));
    }

    public void PassObstacle(MotherGang.Gang gang, MotherGang.GangMember member, Obstacle obstacle, Action SetBasePosition = null)
    {
        //At the end add this member to movables
        //Bu action action.ClimbLadder bitince cagiriliyor. Member i yeniden movable lara ekliyor
        Action addToMovables = delegate () { AddMemberToMovables(gang, member); };
        //Eger member movable ise cikar ordan
        RemoveMemberFromMovables(gang);

        StartCoroutine(actions.PassObstacle(member, obstacle.passStartPosition, obstacle.passEndPosition, addToMovables, SetBasePosition));

    }

    public void AddMemberToMovables(MotherGang.Gang gang, MotherGang.GangMember member)
    {
        //eger liste de degil ise ekle 
        if (gang.MovableMembers.Exists(mem => mem.member == this) == false)
        {
            member.member.SetLock(false);
            gang.MovableMembers.Add(member);

           // if (DataManager.instance.currentGangState == DataManager.GangState.Walking)
             //   member.animator.SetBool("isWalking", true);

            if (gang.AllGang.Exists(mem => mem.member == this) == false)
            {
                gang.AllGang.Add(member);
            }

        }
    }

    //Remove members from movableMembers. If object is dead remove from allmembers as well
    public void RemoveMemberFromMovables(MotherGang.Gang gang, bool didDied = false)
    {
        //eger bu liste de var ise cikar
        if (gang.MovableMembers.Exists(mem => mem.member == this))
        {
            MotherGang.GangMember member = gang.MovableMembers.Find(mem => mem.member == this);

            member.member.SetLock(true);
            
            gang.MovableMembers.Remove(member);

            if(didDied)
            {
                gang.AllGang.Remove(member);
            }
        }
    }

    void WalkAnimation()
    {
        if (isWalking && Vector3.Distance(currPosition, transform.position) <= 0.2f)
        {
            isWalking = false;
            memAnim.SetBool("isWalking", false);
        }                                                                                    //eger y degisiyorsa walking animasyonunu setleme. ya dusuyordur ya da merdiven cikiyordur 
        else if (!isWalking && Vector3.Distance(currPosition, transform.position) > 0.2f) //&& Mathf.Abs(transform.position.y - currPosition.y) < 0.001f)
        {
            memAnim.SetBool("isWalking", true);
            isWalking = true;
        }

        currPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("WreckingBall"))
        {
            RemoveMemberFromMovables(DataManager.instance.GetGang(),true);
            OpenRagdollPhysics();
        }
    }
    
    Vector3 SetNewPosition(Transform gangBase)
    {
        Vector2 currBasePos = new Vector2(gangBase.transform.position.x, gangBase.transform.position.z);

        //eger base in yeri fazla degismediyse member in pozisyonu nu setleme cik
        if (Vector2.Distance(currBasePos, basePosition) < 0.5f)
            return positionInBase;

        Vector2 memberPos = currBasePos + RandomPosInBase;

        positionInBase = new Vector3(memberPos.x, gangBase.position.y - (gangBase.localScale.y) - transform.localScale.y, memberPos.y);
        return positionInBase;
    }

    void MoveTowards(Vector3 posInBase)
    {
        //Calculate the delta
        float delta = Mathf.Abs(Vector3.Distance(transform.localPosition, posInBase)) * 0.1f;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, posInBase, delta);// posVec - gang.Base.transform.position;

        memRb.MovePosition(newPosition);

        Vector3 lookPos = posInBase;
        lookPos.y = transform.position.y;

        transform.LookAt(lookPos);
    }

    public bool SetLock(bool isLocked)
    {
        if( (memberLock && !isLocked) || (!memberLock && isLocked) )
            memberLock = isLocked;

        if (isWalking)
        {
            memAnim.SetBool("isWalking", false);
            isWalking = false;
        }

        return memberLock;
    }

    //baslangicta bi pozisyon setlemek icin
    public Vector2 SetRandomPositionInBase(Transform gangBase)
    {
        RandomPosInBase = (gangBase.localScale.x / 2f * UnityEngine.Random.insideUnitCircle);
        return RandomPosInBase;
    }


}
