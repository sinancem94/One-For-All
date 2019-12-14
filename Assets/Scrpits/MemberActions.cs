using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MemberActions 
{
    float actionSpeed = 2f;
    float RandomAnimationTime()
    {
        return UnityEngine.Random.Range(0.03f, 0.07f);
    }

    public IEnumerator PassObstacle(MotherGang.GangMember member, Vector3 passStartPos, Vector3 passEndPos, Action AddToMovables, Action setNewGangBasePostion = null)
    {
        member.transform.GetComponent<Rigidbody>().useGravity = false;
        member.transform.GetComponent<Collider>().isTrigger = true;

        float randX = UnityEngine.Random.Range(-5f, 5f);
        float randZ = UnityEngine.Random.Range(2f, 10f);

        //if direction is reserved members will go to wrong position due to randZ is always positive. Make it negative if direction is reversed
        if (passStartPos.z > passEndPos.z || passStartPos.y > passEndPos.y)
        {
            randZ *= -1;
        }

        Vector3 lastPos = new Vector3(passEndPos.x + randX, passEndPos.y, passEndPos.z + randZ);

        //send member to ladders start position
        member.animator.SetBool("isWalking", true);
        while (Vector3.SqrMagnitude(member.transform.position - passStartPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, passStartPos, actionSpeed);
            yield return new WaitForSecondsRealtime(RandomAnimationTime());
        }
        member.transform.position = passStartPos;

        //climb member to the top of the ladder
        while (Vector3.SqrMagnitude(member.transform.position - passEndPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, passEndPos, actionSpeed);
            yield return new WaitForSecondsRealtime(RandomAnimationTime());
        }
        member.transform.position = passEndPos;

        //send member to a random location after climbing to prevent them all stay at the same position
        //this position should be handled more precisely
        member.transform.LookAt(lastPos);

        while (Vector3.SqrMagnitude(member.transform.position - lastPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, lastPos, actionSpeed);
            yield return new WaitForSecondsRealtime(RandomAnimationTime());
        }

        member.transform.position = lastPos;

        member.animator.SetBool("isWalking", false);

        member.transform.GetComponent<Rigidbody>().useGravity = true;
        member.transform.GetComponent<Collider>().isTrigger = false;

        if (setNewGangBasePostion != null)
            setNewGangBasePostion();
        if (AddToMovables != null)
            AddToMovables();
    }

    /// <summary>
    /// Called from member itself to set itself as the part of the pass
    /// </summary>
    /// <param name="member"></param>
    /// <param name="passStartPos"></param>
    /// <param name="memberPosInPass"></param>
    /// <param name="lookDirection"></param>
    /// Whether the pas will look downwards or forward. Change according to pass that is building
    /// <param name="setNewGangBasePostion"></param>
    /// <returns></returns>
    public IEnumerator BePassPart(MotherGang.GangMember member, Vector3 passStartPos, Vector3 memberPosInPass ,Action setNewGangBasePostion, int lookDirection = 0)
    {
        member.transform.GetComponent<Rigidbody>().useGravity = false;
        member.transform.GetComponent<Collider>().isTrigger = true;

        //send member to pass start position
        member.animator.SetBool("isWalking", true);

        while (Vector3.SqrMagnitude(member.transform.position - passStartPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, passStartPos, actionSpeed);
            yield return new WaitForSecondsRealtime(RandomAnimationTime());
        }

        member.transform.position = passStartPos;

        //climb member to its corresponding pass position
        member.animator.SetBool("isWalking", false);
        member.animator.SetBool("isClimbing", true);

        
        if (lookDirection != 0)
        {
            Vector3 newLookPos = member.transform.position;
            newLookPos.y += Mathf.Abs(member.transform.position.y * 2) * lookDirection;
            member.transform.LookAt(newLookPos);
        }

        while (Vector3.SqrMagnitude(member.transform.position - memberPosInPass) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, memberPosInPass, actionSpeed);
            yield return new WaitForSecondsRealtime(RandomAnimationTime());
        }

        member.transform.position = memberPosInPass;

        //set the after climb position of the member
        member.animator.SetBool("isClimbFinished", true);
        member.animator.SetBool("isClimbing", false);

        if (setNewGangBasePostion != null)
            setNewGangBasePostion();
    }
}
