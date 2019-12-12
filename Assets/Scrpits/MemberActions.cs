using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MemberActions 
{
    float actionSpeed = 1.5f;

    //Members will call this function for climbing existing ladders. 
    public IEnumerator ClimbLadder(MotherGang.GangMember member, Vector3 ladderStartPos, Vector3 memberPosInLadder, Action AddToMovables, Action setNewGangBasePostion = null)
    {
        member.transform.GetComponent<Rigidbody>().useGravity = false;
        member.transform.GetComponent<Collider>().isTrigger = true;

        float randX = UnityEngine.Random.Range(-5f, 5f);
        float randZ = UnityEngine.Random.Range(2f, 10f);

        Vector3 lastPos = new Vector3(memberPosInLadder.x + randX, memberPosInLadder.y, memberPosInLadder.z + randZ);

        //send member to ladders start position
        member.animator.SetBool("isWalking", true);

        while (Vector3.SqrMagnitude(member.transform.position - ladderStartPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, ladderStartPos, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }

        member.transform.position = ladderStartPos;

        //climb member to the top of the ladder
        member.animator.SetBool("isWalking", false);
        member.animator.SetBool("isClimbing", true);

        while (Vector3.SqrMagnitude(member.transform.position - memberPosInLadder) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, memberPosInLadder, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }

        member.transform.position = memberPosInLadder;
        //send member to a random location after climbing to prevent them all stay at the same position
        //this position should be handled more precisely
        member.animator.SetBool("isClimbing", false);
        member.animator.SetBool("isWalking", true);
        while (Vector3.SqrMagnitude(member.transform.position - lastPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, lastPos, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }

        member.transform.position = lastPos;

        member.animator.SetBool("isWalking", false);
        member.transform.GetComponent<Rigidbody>().useGravity = true;
        member.transform.GetComponent<Collider>().isTrigger = false;

        //eger member dolu geldiyse (ki gelmeli error basilabilir.) movable lara ekle
        if (AddToMovables != null)
            AddToMovables();

        //eger bu obje belirlenen sirada cikan adamsa base pozisyonu ata
        if (setNewGangBasePostion != null)
            setNewGangBasePostion();
    }

    //This function is called from member to create ladder.
    public IEnumerator CreateLadder(MotherGang.GangMember member, Vector3 ladderStartPos, Vector3 memberPosInLadder, Action setNewGangBasePostion)
    {
        member.transform.GetComponent<Rigidbody>().useGravity = false;
        member.transform.GetComponent<Collider>().isTrigger = true;

        //send member to ladders start position
        member.animator.SetBool("isWalking", true);

        while (Vector3.SqrMagnitude(member.transform.position - ladderStartPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, ladderStartPos, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }
        member.transform.position = ladderStartPos;

        //climb member to its corresponding ladder position
        member.animator.SetBool("isWalking", false);
        member.animator.SetBool("isClimbing", true);

        while (Vector3.SqrMagnitude(member.transform.position - memberPosInLadder) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, memberPosInLadder, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }

        member.transform.position = memberPosInLadder;

        //set the after climb position of the member
        member.animator.SetBool("isClimbFinished", true);
        member.animator.SetBool("isClimbing", false);
        
        if (setNewGangBasePostion != null)
            setNewGangBasePostion();
    }


    public IEnumerator CreateBridge(MotherGang.GangMember member, Vector3 bridgeStartPos, Vector3 memberPosInBridge, Action setNewGangBasePostion)
    {
        member.transform.GetComponent<Rigidbody>().useGravity = false;
        member.transform.GetComponent<Collider>().isTrigger = true;

        //send member to ladders start position
        member.transform.LookAt(bridgeStartPos);
        member.animator.SetBool("isWalking", true);
        while (Vector3.SqrMagnitude(member.transform.position - bridgeStartPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, bridgeStartPos, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }

        member.transform.position = bridgeStartPos;

        //climb member to its corresponding ladder position
        Vector3 newLookPos = member.transform.position;
        newLookPos.y -= 5;
        member.transform.LookAt(newLookPos);
        member.animator.SetBool("isWalking", false);
        member.animator.SetBool("isClimbing", true);
        while (Vector3.SqrMagnitude(member.transform.position - memberPosInBridge) > 0.5f)
        {
            newLookPos = member.transform.position;
            newLookPos.y -= 5;
            member.transform.LookAt(newLookPos);
            member.transform.position = Vector3.MoveTowards(member.transform.position, memberPosInBridge, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }

        member.transform.position = memberPosInBridge;

        //set the after climb position of the member
        member.animator.SetBool("isClimbFinished", true);
        member.animator.SetBool("isClimbing", false);
        newLookPos = member.transform.position;
        newLookPos.y -= 5;
        member.transform.LookAt(newLookPos);

        if(setNewGangBasePostion != null)
            setNewGangBasePostion();
    }

    public IEnumerator PassBridge(MotherGang.GangMember member, Vector3 bridgeStartPos, Vector3 memberPosInBridge, Action AddToMovables, Action setNewGangBasePostion = null)
    {
        member.transform.GetComponent<Rigidbody>().useGravity = false;
        member.transform.GetComponent<Collider>().isTrigger = true;


        float randX = UnityEngine.Random.Range(-5f, 5f);
        float randZ = UnityEngine.Random.Range(2f, 10f);

        Vector3 lastPos = new Vector3(memberPosInBridge.x + randX, memberPosInBridge.y, memberPosInBridge.z + randZ);

        //send member to ladders start position
        member.animator.SetBool("isWalking", true);
        while (Vector3.SqrMagnitude(member.transform.position - bridgeStartPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, bridgeStartPos, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }
        member.transform.position = bridgeStartPos;

        //climb member to the top of the ladder
        while (Vector3.SqrMagnitude(member.transform.position - memberPosInBridge) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, memberPosInBridge, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }
        member.transform.position = memberPosInBridge;

        //send member to a random location after climbing to prevent them all stay at the same position
        //this position should be handled more precisely
        member.transform.LookAt(lastPos);

        while (Vector3.SqrMagnitude(member.transform.position - lastPos) > 0.5f)
        {
            member.transform.position = Vector3.MoveTowards(member.transform.position, lastPos, actionSpeed);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.1f));
        }

        member.transform.position = lastPos;

        member.animator.SetBool("isWalking", false);
        member.transform.GetComponent<Rigidbody>().useGravity = true;
        member.transform.GetComponent<Collider>().isTrigger = false;

        if (AddToMovables != null)
            AddToMovables();
        if (setNewGangBasePostion != null)
            setNewGangBasePostion();
    }
}
