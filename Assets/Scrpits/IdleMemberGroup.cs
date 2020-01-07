using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleMemberGroup : MonoBehaviour
{
    public int memberCount;

    List<Member> IdleMembers;

    Transform IdleBase;

    void Start()
    {
        Vector3 groundPosition;
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, LayerMask.NameToLayer("Ground")))
        {
            groundPosition = hit.transform.position;

            groundPosition.y += hit.transform.localScale.y / 2f;
        }
        else
        {
           // Debug.LogError("Idle Members are not on any ground.");
            Transform ground = GameObject.FindGameObjectWithTag("Ground").transform;

            groundPosition = ground.position;

            groundPosition.y += ground.localScale.y / 2f;
        }

        transform.position = new Vector3(transform.position.x,groundPosition.y,transform.position.z);

        SetBase();
        CreateMembers();
    }

    void CreateMembers()
    {
        IdleMembers = new List<Member>(memberCount);
        List<Transform> IdleMemTrans = ObjectPooler.instance.PooltheObjects(DataManager.instance.levelData.memberToLoad, memberCount, this.transform, true);

        for (int i = 0; i < memberCount; i++)
        {
            Member member = IdleMemTrans[i].GetComponent<Member>();

            Vector2 basePos = new Vector2(IdleBase.position.x, IdleBase.position.z);
            Vector2 memberPos = basePos + member.SetRandomPositionInBase(IdleBase);

            member.transform.position = new Vector3(memberPos.x, 0f, memberPos.y);

            member.SetLock(true);

            IdleMembers.Add(member);
        }
    }


    //Bekleyen memberlarin altinda durcagi bir base. Bu carpiscak gang ile
    void SetBase()
    {
        float radius = DataManager.instance.levelData.memberToLoad.localScale.x * memberCount / 1.5f;

        radius = (radius < 7f) ? 7f : radius;

        Transform gangBase = transform.GetChild(0);
        gangBase.localScale = new Vector3(radius, radius / 2f, radius);
        gangBase.localPosition = new Vector3(gangBase.transform.localPosition.x, gangBase.transform.localPosition.y + (gangBase.localScale.y), gangBase.transform.localPosition.z);

        IdleBase = gangBase;
    }


    public void AddMembersToGang()
    {
        string gangMemberTag = "GangMember";

        foreach(Member m in IdleMembers)
        {
            m.gameObject.tag = gangMemberTag;
            m.gameObject.layer = LayerMask.NameToLayer(gangMemberTag);

            m.transform.parent = DataManager.instance.GetGang().Base.parent;

            MotherGang.GangMember gangMember = new MotherGang.GangMember(m.transform,m);

            m.AddMemberToMovables(gangMember);
        }
    }

    void DestroyMe()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Base"))
        {
            AddMembersToGang();
            DestroyMe();
        }
    }
}
