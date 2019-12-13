using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Member : MonoBehaviour
{
    //member larin harekerini saglamak icin mutex. eger tirmanma veya kopru gecme vs islemindeyse lock la yoksa acik dursun
    public bool memberLock;

    MemberActions actions;

    //ilk basta MotherGang de setleniyor
    Vector3 positionInBase;
    Vector2 RandomPosInBase;

    //basePos her framede updateleniyor eger farkli ise positionInBase degistiriliyor
    Vector2 basePosition;

    void Start()
    {
        memberLock = false;
        actions = new MemberActions();
        basePosition = Vector2.one * 10f;

        CloseRagdollPhysics();
    }

    private void Update()
    {
        SetNewPosition(DataManager.instance.GetGang().Base);
        if (!memberLock && (DataManager.instance.currentGangState == MotherGang.GangState.Walking || DataManager.instance.currentGangState == MotherGang.GangState.Idle))
        {
            MoveTowards(positionInBase);
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

    #region LadderClimbing
    //Bunlari ayirdim cunku bazen sadece merdiven tirmandirmak isticegimiz durumlar olabilir
    public void CreateLadder(MotherGang.Gang gang, Vector3 ladderPos, Vector3 positionInLadder, Action SetBasePosition = null)
    {
        MotherGang.GangMember member = gang.AllGang.Find(mem => mem.member == this);
        //Eger member movable ise cikar ordan
        RemoveMemberFromMovables(gang);

        StartCoroutine(actions.CreateLadder(member, ladderPos, positionInLadder, SetBasePosition));
    }

    public void ClimbLadder(MotherGang.Gang gang, Vector3 ladderPos, Vector3 ladderEndPosition, Action SetBasePosition = null)
    {
        MotherGang.GangMember member = gang.AllGang.Find(mem => mem.member == this);

        //At the end add this member to movables
        //Bu action action.ClimbLadder bitince cagiriliyor. Member i yeniden movable lara ekliyor
        Action addToMovables = delegate () { AddMemberToMovables(gang,member);};

        //Eger member movable ise cikar ordan
        RemoveMemberFromMovables(gang);

        StartCoroutine(actions.ClimbLadder(member, ladderPos, ladderEndPosition, addToMovables, SetBasePosition));
    }
    #endregion


    #region BridgePassing
    //Bunlari ayirdim cunku bazen sadece kopru gecirmek gerekli durumlar olabilir
    public void CreateBridge(MotherGang.Gang gang, Vector3 bridgePos, Vector3 positionInBridge, Action SetBasePosition = null)
    {
        MotherGang.GangMember member = gang.AllGang.Find(mem => mem.member == this);

        RemoveMemberFromMovables(gang);

        StartCoroutine(actions.CreateBridge(member, bridgePos, positionInBridge, SetBasePosition));
    }

    public void PassBridge(MotherGang.Gang gang, Vector3 bridgePos, Vector3 bridgeEndPosition, Action SetBasePosition = null)
    {
        MotherGang.GangMember member = gang.AllGang.Find(mem => mem.member == this);

        //At the end add this member to movables
        Action addToMovables = delegate () { AddMemberToMovables(gang,member); };

        RemoveMemberFromMovables(gang);

        StartCoroutine(actions.PassBridge(member, bridgePos, bridgeEndPosition, addToMovables, SetBasePosition));
    }
    #endregion

    public void AddMemberToMovables(MotherGang.Gang gang)
    {
        //eger liste de degil ise ekle 
        if (gang.MovableMembers.Exists(mem => mem.member == this) == false)
        {
            memberLock = false;

            MotherGang.GangMember member = gang.AllGang.Find(mem => mem.member == this);
            gang.MovableMembers.Add(member);

            if (DataManager.instance.currentGangState == MotherGang.GangState.Walking)
                member.animator.SetBool("isWalking", true);

            if(gang.AllGang.Exists(mem => mem.member == this) == false)
            {
                gang.AllGang.Add(member);
            }

        }
    }

    public void AddMemberToMovables(MotherGang.Gang gang, MotherGang.GangMember member)
    {
        //eger liste de degil ise ekle 
        if (gang.MovableMembers.Exists(mem => mem.member == this) == false)
        {
            memberLock = false;
            gang.MovableMembers.Add(member);

            if (DataManager.instance.currentGangState == MotherGang.GangState.Walking)
                member.animator.SetBool("isWalking", true);

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
            memberLock = true;

            MotherGang.GangMember member = gang.MovableMembers.Find(mem => mem.member == this);
            gang.MovableMembers.Remove(member);

            if(didDied)
            {
                gang.AllGang.Remove(member);
            }
        }
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

        //eger degismediyse fazla setleme cik
        if (Vector2.Distance(currBasePos, basePosition) < 0.5f)
            return positionInBase;

        Vector2 memberPos = currBasePos + RandomPosInBase;

        positionInBase = new Vector3(memberPos.x, gangBase.localPosition.y - (gangBase.localScale.y), memberPos.y);
        return positionInBase;
    }

    void MoveTowards(Vector3 posInBase)
    {
        //Calculate the delta
        float delta = Mathf.Abs(Vector3.Distance(transform.localPosition, posInBase)) * 0.09f;

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, posInBase, delta);

        Vector3 lookPos = posInBase;
        lookPos.y = transform.position.y;

        transform.LookAt(lookPos);
    }

    //baslangicta bi pozisyon setlemek icin
    public Vector2 SetRandomPositionInBase(Transform gangBase)
    {
        RandomPosInBase = (gangBase.localScale.x / 2f * UnityEngine.Random.insideUnitCircle);
        return RandomPosInBase;
    }

}
