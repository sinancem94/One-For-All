using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MemberActions 
{
    float actionSpeed = 2f;
    public Coroutine memberEvent;

    float RandomAnimationTime()
    {
        return UnityEngine.Random.Range(0.03f, 0.07f);
    }


    public void PassObstacle(MonoBehaviour owner, MotherGang.GangMember member, Vector3 passStartPos, Vector3 passEndPos, Action setNewGangBasePostion = null)
    {
        if (memberEvent != null)
        {
            owner.StopCoroutine(memberEvent);
            memberEvent = null;
        }

        memberEvent = owner.StartCoroutine(PassObstacleCorountine(member,passStartPos,passEndPos,setNewGangBasePostion));

    }

    public void BePassPart(MonoBehaviour owner, MotherGang.GangMember gangMem, Vector3 passStartPos, Vector3 memberPosInPass, Action setNewGangBasePostion, int lookDirection, Action removeMeFromGang)
    {
        if (memberEvent != null)
        {
            owner.StopCoroutine(memberEvent);
            memberEvent = null;
        }

        memberEvent = owner.StartCoroutine(BePassPartCorountine(gangMem, passStartPos, memberPosInPass, setNewGangBasePostion, lookDirection, removeMeFromGang));
    }

    IEnumerator PassObstacleCorountine(MotherGang.GangMember gangMem, Vector3 passStartPos, Vector3 passEndPos, Action setNewGangBasePostion = null)
    {
        gangMem.member.memRb.useGravity = false;
        gangMem.transform.GetComponent<Collider>().isTrigger = true;

        float lastZPositionModifier = 4f;

        //if direction is reserved members will go to wrong position due to randZ is always positive. Make it negative if direction is reversed
        if (passStartPos.z > passEndPos.z || passStartPos.y > passEndPos.y)
        {
          lastZPositionModifier *= -1;
        }

        Vector3 lastPos = new Vector3(passEndPos.x, passEndPos.y, passEndPos.z + lastZPositionModifier);

        gangMem.member.memAnim.SetBool("isWalking", true);

        //eger bitise daha yakinsa oraya dogru git ilk basta
        if(Vector3.Distance(gangMem.transform.position,passStartPos) < Vector3.Distance(gangMem.transform.position, passEndPos))
        {
            //send member to ladders start position
            while (Vector3.SqrMagnitude(gangMem.transform.position - passStartPos) > 0.5f)
            {
                gangMem.transform.position = Vector3.MoveTowards(gangMem.transform.position, passStartPos, actionSpeed);
                yield return new WaitForSecondsRealtime(RandomAnimationTime());
            }
            gangMem.transform.position = passStartPos;
        }
       

        //climb member to the top of the ladder
        while (Vector3.SqrMagnitude(gangMem.transform.position - passEndPos) > 0.5f)
        {
            gangMem.transform.position = Vector3.MoveTowards(gangMem.transform.position, passEndPos, actionSpeed);
            yield return new WaitForSecondsRealtime(RandomAnimationTime());
        }
        gangMem.transform.position = passEndPos;

        //tirmandiktan sonra base asagidaysa yukari tasi
        if (setNewGangBasePostion != null)
            setNewGangBasePostion();

        //send member a litle bit further in order to avoid collider issues. Member will automatically go to it`s assigned position after added to movables list.
        gangMem.transform.LookAt(passEndPos);

        while (Vector3.SqrMagnitude(gangMem.transform.position - lastPos) > 0.5f)
        {
            gangMem.transform.position = Vector3.MoveTowards(gangMem.transform.position, lastPos, actionSpeed);
            yield return new WaitForSecondsRealtime(RandomAnimationTime());
        }

        gangMem.member.memAnim.SetBool("isWalking", false);

        gangMem.transform.GetComponent<Rigidbody>().useGravity = true;
        gangMem.transform.GetComponent<Collider>().isTrigger = false;

        //bu member i movable a ekle
        gangMem.member.AddMemberToMovables(gangMem);
    }

    /// <summary>
    /// Called from member itself to set itself as the part of the pass
    /// </summary>
    /// <param name="gangMem"></param>
    /// <param name="passStartPos"></param>
    /// <param name="memberPosInPass"></param>
    /// <param name="lookDirection"></param>
    /// Whether the pas will look downwards or forward. Change according to pass that is building
    /// <param name="setNewGangBasePostion"></param>
    /// <returns></returns>
    IEnumerator BePassPartCorountine(MotherGang.GangMember gangMem, Vector3 passStartPos, Vector3 memberPosInPass ,Action setNewGangBasePostion, int lookDirection, Action removeMeFromGang)
    {
        gangMem.member.memRb.useGravity = false;
        gangMem.transform.GetComponent<Collider>().enabled = false;

        //send member to pass start position
        gangMem.member.memAnim.SetBool("isWalking", true);

        while (Vector3.SqrMagnitude(gangMem.transform.position - passStartPos) > 0.5f)
        {
            gangMem.transform.position = Vector3.MoveTowards(gangMem.transform.position, passStartPos, actionSpeed);
            yield return new WaitForSecondsRealtime(RandomAnimationTime());
        }

        gangMem.transform.position = passStartPos;

        //climb member to its corresponding pass position
        gangMem.member.memAnim.SetBool("isWalking", false);
        gangMem.member.memAnim.SetBool("isClimbing", true);

        
        if (lookDirection != 0)
        {
            Vector3 newLookPos = gangMem.transform.position;
            newLookPos.y += Mathf.Abs(gangMem.transform.position.y * 2) * lookDirection;
            gangMem.transform.LookAt(newLookPos);
        }

        while (Vector3.SqrMagnitude(gangMem.transform.position - memberPosInPass) > 0.5f)
        {
            gangMem.transform.position = Vector3.MoveTowards(gangMem.transform.position, memberPosInPass, actionSpeed);
            yield return new WaitForSecondsRealtime(RandomAnimationTime());
        }

        gangMem.transform.position = memberPosInPass;

        //set the after climb position of the member
        gangMem.member.memAnim.SetBool("isClimbFinished", true);
        gangMem.member.memAnim.SetBool("isClimbing", false);

        gangMem.member.memRb.isKinematic = true;

        if (setNewGangBasePostion != null)
            setNewGangBasePostion();

        if (removeMeFromGang != null)
            removeMeFromGang();
    }
}
