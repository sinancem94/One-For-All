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

    //for walking animation. if memberLock is UNLOCKED Member will start walk animation automaticaly if position changed. 
    Vector3 currPosition;
    bool isWalking = false;

    List<Transform> pushingSquashers;

    

    void Start()
    {
        memRb = GetComponent<Rigidbody>();
        memAnim = GetComponent<Animator>();

        actions = new MemberActions();

        CloseRagdollPhysics();

        pushingSquashers = new List<Transform>();
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

            if (DidWalked() && !isWalking)
                StartWalkingAnimation();
            else if (!DidWalked() && isWalking)
                StopWalkingAnimation();

            currPosition = transform.position;
        }

     //   if(memRb.velocity.magnitude > 0f)
       //     memRb.velocity = Vector3.zero;
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
        //Eger member movable ise cikar ordan
        RemoveMemberFromMovables(gang);
        //if bridge set as -1 . member will look downwards
        //if ladder set as 0. will not call LookAt for member 
        //if otherthere is a error set as 0
        int lookDirection = (obstacle.ObstacleType == Obstacle.Type.Bridge) ? -1 : (obstacle.ObstacleType == Obstacle.Type.Ladder) ? 0 : 0;

        Action removeMeFromGang = delegate () {
            DestroyMe();
        };
        actions.BePassPart(this, member, passStartPos, memberPosInPass, SetBasePosition, lookDirection, removeMeFromGang);
        //StartCoroutine(actions.BePassPart(member, passStartPos, memberPosInPass, SetBasePosition, lookDirection,removeMeFromGang));
    }

    public void PassObstacle(MotherGang.Gang gang, MotherGang.GangMember member, Obstacle obstacle, Action SetBasePosition = null)
    {
        //Eger member movable ise cikar ordan. PassObstacle sonunda yine eklencek
        RemoveMemberFromMovables(gang);

        //At the end add this member to movables
        actions.PassObstacle(this, member, obstacle.passStartPosition, obstacle.passEndPosition, SetBasePosition);
        //StartCoroutine(actions.PassObstacleCorountine(member, obstacle.passStartPosition, obstacle.passEndPosition, addToMovables, SetBasePosition));
    }

    public void AddMemberToMovables(MotherGang.GangMember member)
    {
        //eger liste de degil ise ekle 
        if (DataManager.instance.GetGang().MovableMembers.Exists(mem => mem.member == this) == false)
        {
            member.member.SetLock(false);
            DataManager.instance.GetGang().MovableMembers.Add(member);

            //eger all gangde de yok ise ona da ekle
            if (DataManager.instance.GetGang().AllGang.Exists(mem => mem.member == this) == false)
            {
                DataManager.instance.GetGang().AllGang.Add(member);
            }

        }
    }

    //Remove members from movableMembers. If object is dead remove from allmembers as well
    public void RemoveMemberFromMovables(MotherGang.Gang gang)
    {
        //eger bu liste de var ise cikar
        if (gang.MovableMembers.Exists(mem => mem.member == this))
        {
            MotherGang.GangMember member = gang.MovableMembers.Find(mem => mem.member == this);
            member.member.SetLock(true);
            gang.MovableMembers.Remove(member);
        }
    }

    //walking animation olcak mi diye
    bool DidWalked()
    {
        //sadece x ve z pozisyonlarini karsilastir. eger sadece y degisiyorsa yurume animasyonu calistirmaya gerek yok
        Vector2 currPositionInPlane = new Vector2(currPosition.x, currPosition.z);
        Vector2 changedPositionInPlane = new Vector2(transform.position.x, transform.position.z);

        if (Vector2.Distance(currPositionInPlane, changedPositionInPlane) > 0.2f) //&& Mathf.Abs(transform.position.y - currPosition.y) < 0.001f)
        {
            return true;
        }

        return false;
    }

    void StartWalkingAnimation()
    {
        isWalking = true;
        memAnim.SetBool("isWalking", true);
        GetComponentInChildren<ParticleSystem>().Play();
    }

    void StopWalkingAnimation()
    {
        isWalking = false;
        memAnim.SetBool("isWalking", false);
        GetComponentInChildren<ParticleSystem>().Stop();
    }

    void StartPushingAnimation()
    {
        memAnim.SetBool("isPushing", true);
    }

    void StopPushingAnimation()
    {
        memAnim.SetBool("isPushing", false);
    }

    //eger olduyse cagir beni
    void DestroyMe()
    {
        StopAllCoroutines();
        SetLock(true);

        //eger bu liste de var ise cikar
        if (DataManager.instance.GetGang().MovableMembers.Exists(mem => mem.member == this))
        {
            MotherGang.GangMember member = DataManager.instance.GetGang().MovableMembers.Find(mem => mem.member == this);

            DataManager.instance.GetGang().MovableMembers.Remove(member);
        }

        if (DataManager.instance.GetGang().AllGang.Exists(mem => mem.member == this))
        {
            MotherGang.GangMember member = DataManager.instance.GetGang().AllGang.Find(mem => mem.member == this);

            DataManager.instance.GetGang().AllGang.Remove(member);
        }

        this.GetComponent<Member>().enabled = false;
    }
    
    Vector3 SetNewPosition(Transform gangBase)
    {
        Vector2 currBasePos = new Vector2(gangBase.transform.position.x, gangBase.transform.position.z);
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

    private void OnCollisionEnter(Collision collision)
    {
        Collider otherCollider = collision.contacts[0].otherCollider;

        if (otherCollider.gameObject.CompareTag("WreckingBall"))
        {
            DestroyMe();
            OpenRagdollPhysics();
        }
        else if(otherCollider.gameObject.CompareTag("Squasher"))
        {
            if (pushingSquashers.Count == 0 || pushingSquashers.Exists(trans => trans.GetInstanceID() == otherCollider.transform.GetInstanceID()) == false)
            {
                pushingSquashers.Add(otherCollider.transform);
            }

            if (pushingSquashers.Count == 2)
            {
                DestroyMe();
                OpenRagdollPhysics();
            }

        }
        else if (otherCollider.gameObject.CompareTag("Wall") || otherCollider.gameObject.CompareTag("PushableObject"))
        {
            RandomPosInBase = DataManager.instance.GetGang().Base.position -  transform.position; //SetRandomPositionInBase(DataManager.instance.GetGang().Base);
            StartPushingAnimation();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Collider otherCollider = collision.contacts[0].otherCollider;

        if (otherCollider.gameObject.CompareTag("Squasher"))
        {
            RandomPosInBase = DataManager.instance.GetGang().Base.position - transform.position;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("PushableObject"))
        {
            RandomPosInBase = SetRandomPositionInBase(DataManager.instance.GetGang().Base);
            StopPushingAnimation();
        }
        else if(collision.gameObject.CompareTag("Squasher"))
        {
            if (pushingSquashers.Exists(trans => trans.GetInstanceID() == collision.collider.transform.GetInstanceID()))
            {
                pushingSquashers.Remove(collision.collider.transform);
            }

            RandomPosInBase = SetRandomPositionInBase(DataManager.instance.GetGang().Base);
        }
       
    }
}
