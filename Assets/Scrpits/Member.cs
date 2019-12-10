using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Member : MonoBehaviour
{
    MemberActions actions;

    //ilk basta MotherGang de setleniyor
    Vector3 positionInBase;
    Vector2 RandomPosInBase;

    void Start()
    {
        actions = new MemberActions();

        CloseRagdollPhysics();
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

    //Bunlari ayirdim cunku bazen sadece merdiven tirmandirmak isticegimiz durumlar olabilir
    public void CreateLadder(MotherGang.Gang gang, bool isLadder, Vector3 ladderPos, Vector3 positionInLadder, Action SetBasePosition = null)
    {
        MotherGang.GangMember member = gang.AllGang.Find(mem => mem.member == this);

        if (isLadder)
        {
            BeLadder(gang, member, ladderPos, positionInLadder,SetBasePosition);
        }
        else
        {
            ClimbLadder(gang, member, ladderPos, positionInLadder);
        }
    }

    void BeLadder(MotherGang.Gang gang, MotherGang.GangMember member, Vector3 ladderPos, Vector3 positionInLadder, Action SetBasePosition)
    {
        //Eger member movable ise cikar ordan
        RemoveMemberFromMovables(gang);

        StartCoroutine(actions.CreateLadder(member, ladderPos, positionInLadder, SetBasePosition));
    }

    public void ClimbLadder(MotherGang.Gang gang, MotherGang.GangMember member, Vector3 ladderPos, Vector3 positionInLadder)
    {
        //At the end add this member to movables
        //Bu action action.ClimbLadder bitince cagiriliyor. Member i yeniden movable lara ekliyor
        Action addToMovables = delegate () { AddMemberToMovables(gang); };

        //Eger member movable ise cikar ordan
        RemoveMemberFromMovables(gang);

        StartCoroutine(actions.ClimbLadder(member, ladderPos, positionInLadder,addToMovables));
    }


    //Bunlari ayirdim cunku bazen sadece kopru gecirmek isticegimiz durumlar olabilir
    public void CreateBridge(MotherGang.Gang gang, bool isBridge, Vector3 bridgePos, Vector3 positionInBridge, Action SetBasePosition = null)
    {
        MotherGang.GangMember member = gang.AllGang.Find(mem => mem.member == this);

        if (isBridge)
        {
            BeBridge(gang, member, bridgePos, positionInBridge, SetBasePosition);
        }
        else
        {
            PassBridge(gang, member, bridgePos, positionInBridge);
        }
    }

    void BeBridge(MotherGang.Gang gang, MotherGang.GangMember member, Vector3 ladderPos, Vector3 positionInLadder, Action SetBasePosition)
    {
        RemoveMemberFromMovables(gang);

        StartCoroutine(actions.CreateBridge(member, ladderPos, positionInLadder,SetBasePosition));
    }

    void PassBridge(MotherGang.Gang gang, MotherGang.GangMember member, Vector3 ladderPos, Vector3 positionInLadder)
    {
        //At the end add this member to movables
        Action addToMovables = delegate () { AddMemberToMovables(gang); };

        RemoveMemberFromMovables(gang);

        StartCoroutine(actions.PassBridge(member, ladderPos, positionInLadder, addToMovables));
    }


    public void AddMemberToMovables(MotherGang.Gang gang)
    {
        //eger liste de degil ise ekle 
        if (gang.MovableMembers.Exists(mem => mem.member == this) == false)
        {
            MotherGang.GangMember member = gang.AllGang.Find(mem => mem.member == this);
            gang.MovableMembers.Add(member);
        }
    }

    public void RemoveMemberFromMovables(MotherGang.Gang gang)
    {
        //eger bu liste de var ise cikar
        if (gang.MovableMembers.Exists(mem => mem.member == this))
        {
            MotherGang.GangMember member = gang.MovableMembers.Find(mem => mem.member == this);
            gang.MovableMembers.Remove(member);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("WreckingBall"))
        {
            Debug.Log("wreckk");
            OpenRagdollPhysics();
        }
    }
    
    Vector3 SetNewPosition(Transform gangBase)
    {
        Vector2 basePos = new Vector2(gangBase.transform.position.x, gangBase.transform.position.z);
        Vector2 memberPos = basePos + RandomPosInBase;

        positionInBase = new Vector3(memberPos.x, 0f , memberPos.y);
        return positionInBase;
    }

    public void MoveTowards(Transform gangBase)
    {
        positionInBase = SetNewPosition(gangBase);

        float delta = Vector2.Distance(transform.localPosition, positionInBase) * 0.5f;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, positionInBase,0.5f);

        Vector3 lookPos = positionInBase;
        lookPos.y = transform.position.y;

        transform.LookAt(lookPos);
    }

    //baslangicta bi pozisyon setlemek icin
    public Vector2 SetPosInBase(Transform gangBase)
    {
        RandomPosInBase = (gangBase.localScale.x / 2f * UnityEngine.Random.insideUnitCircle);
        return RandomPosInBase;
    }

}
