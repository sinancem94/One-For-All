using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberActions : MonoBehaviour
{
    Animator animator;

    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public IEnumerator CreateLadder(bool isPartOfTheLadder, Vector3 ladderStartPos, Vector3 memberPosInLadder, Transform lookPosition)
    {
        DataScript.isGravityOpen = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().isTrigger = true;
        if (isPartOfTheLadder)
        {
            //send member to ladders start position
            transform.LookAt(ladderStartPos);
            animator.SetBool("isWalking", true);
            while (Vector3.SqrMagnitude(transform.position - ladderStartPos) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, ladderStartPos, 0.5f);
                yield return new WaitForSecondsRealtime(0.02f);
            }

            //climb member to its corresponding ladder position
            transform.LookAt(lookPosition);
            animator.SetBool("isWalking", false);
            animator.SetBool("isClimbing", true);

            while (Vector3.SqrMagnitude(transform.position - memberPosInLadder) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, memberPosInLadder, 1f);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            //set the after climb position of the member
            animator.SetBool("isClimbFinished", true);
            animator.SetBool("isClimbing", false);
            Vector3 newLookPos = lookPosition.position;
            newLookPos.y = transform.position.y;
            transform.LookAt(newLookPos);
            transform.parent = null;
        }
        else
        {
            float randX = Random.Range(-5f, 5f);
            float randZ = Random.Range(2f, 10f);

            Vector3 lastPos = new Vector3(memberPosInLadder.x + randX, memberPosInLadder.y, memberPosInLadder.z + randZ);

            //send member to ladders start position
            transform.LookAt(ladderStartPos);
            animator.SetBool("isWalking", true);
            while (Vector3.SqrMagnitude(transform.position - ladderStartPos) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, ladderStartPos, 2f);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            //climb member to the top of the ladder
            transform.LookAt(lookPosition);        //change it to dynamic
            animator.SetBool("isWalking", false);
            animator.SetBool("isClimbing", true);
            while (Vector3.SqrMagnitude(transform.position - memberPosInLadder) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, memberPosInLadder, 1f);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            //send member to a random location after climbing to prevent them all stay at the same position
            //this position should be handled more precisely
            transform.LookAt(lastPos);
            animator.SetBool("isClimbing", false);
            animator.SetBool("isWalking", true);
            while (Vector3.SqrMagnitude(transform.position - lastPos) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, lastPos, 2f);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            animator.SetBool("isWalking", false);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Collider>().isTrigger = false;
        }

        StopCoroutine(CreateLadder(isPartOfTheLadder, ladderStartPos, memberPosInLadder, lookPosition));
    }

    public IEnumerator CreateBridge(bool isPartOfTheBridge, Vector3 bridgeStartPos, Vector3 memberPosInBridge, Transform lookPosition)
    {

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().isTrigger = true;
        if (isPartOfTheBridge)
        {
            //send member to ladders start position
            transform.LookAt(bridgeStartPos);
            animator.SetBool("isWalking", true);
            while (Vector3.SqrMagnitude(transform.position - bridgeStartPos) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, bridgeStartPos, 0.5f);
                yield return new WaitForSecondsRealtime(0.02f);
            }

            //climb member to its corresponding ladder position
            Vector3 newLookPos = transform.position;
            newLookPos.y -= 5;
            transform.LookAt(newLookPos);
            animator.SetBool("isWalking", false);
            animator.SetBool("isClimbing", true);
            while (Vector3.SqrMagnitude(transform.position - memberPosInBridge) > 0.5f)
            {
                newLookPos = transform.position;
                newLookPos.y -= 5;
                transform.LookAt(newLookPos);
                transform.position = Vector3.MoveTowards(transform.position, memberPosInBridge, 1f);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            //set the after climb position of the member
            animator.SetBool("isClimbFinished", true);
            animator.SetBool("isClimbing", false);
            newLookPos = transform.position;
            newLookPos.y -= 5;
            transform.LookAt(newLookPos);
            transform.parent = null;
        }
        else
        {
            float randX = Random.Range(-5f, 5f);
            float randZ = Random.Range(2f, 10f);

            Vector3 lastPos = new Vector3(memberPosInBridge.x + randX, memberPosInBridge.y, memberPosInBridge.z + randZ);

            //send member to ladders start position
            transform.LookAt(bridgeStartPos);
            animator.SetBool("isWalking", true);
            while (Vector3.SqrMagnitude(transform.position - bridgeStartPos) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, bridgeStartPos, 2f);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            //climb member to the top of the ladder
            Vector3 newLookPos = transform.position;
            newLookPos.y -= 5;
            transform.LookAt(newLookPos);
            animator.SetBool("isWalking", false);
            animator.SetBool("isClimbing", true);
            while (Vector3.SqrMagnitude(transform.position - memberPosInBridge) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, memberPosInBridge, 1f);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            //send member to a random location after climbing to prevent them all stay at the same position
            //this position should be handled more precisely
            transform.LookAt(lastPos);
            animator.SetBool("isClimbing", false);
            animator.SetBool("isWalking", true);
            while (Vector3.SqrMagnitude(transform.position - lastPos) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, lastPos, 2f);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            animator.SetBool("isWalking", false);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Collider>().isTrigger = false;
        }

        StopCoroutine(CreateLadder(isPartOfTheBridge, bridgeStartPos, memberPosInBridge, lookPosition));
    }
}
