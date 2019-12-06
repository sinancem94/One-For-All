﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;

public class GangMovementScript : MonoBehaviour
{
    private List<Animator> gangAnimators;
    public List<Transform> gangTransforms;
    Vector2 initialPos;

    InputX inputX;

    private UIScript uIScript;
    private CameraScript cameraScript;

    private void Start()
    {
        cameraScript = FindObjectOfType(typeof(CameraScript)) as CameraScript;
        uIScript = FindObjectOfType(typeof(UIScript)) as UIScript;
        inputX = new InputX();
        initialPos = new Vector2();
        gangAnimators = new List<Animator>();
        gangTransforms = new List<Transform>();

        SetGangList();
       
    }

    void Update()
    {

        if(transform.childCount == 0)
        {
            uIScript.GameOver();
            Debug.Log("Game Over");
        }
        else
        {
            if (inputX.IsInput() && !DataScript.inputLock)
            {
                GeneralInput gInput = inputX.GetInput(0);
                MoveTheGang(gInput);
            }

            else if (!DataScript.inputLock)      //this input lock is to make walking possible in ladder or bridge creating processes
            {
                foreach (Animator gangMemberAnim in gangAnimators)
                {
                    gangMemberAnim.SetBool("isWalking", false);
                }
            }
        }

        
    }

    public void MoveTheGang(GeneralInput input)
    {
        if(input.phase == IPhase.Began)
        {
            initialPos = input.currentPosition;
        }
       
        else
        {
            
            foreach (Animator gangMemberAnim in gangAnimators)
            {
               
                gangMemberAnim.SetBool("isWalking", true);
            }

            Vector2 toPos = input.currentPosition;
            Vector2 diffVec = toPos - initialPos;
            
            //transform.rotation = Quaternion.identity;


            Vector3 posVec = transform.position;
            posVec.x += diffVec.x;
            posVec.z += diffVec.y;
            Vector3 lookPos = posVec;
            lookPos.y = gangTransforms[0].position.y;

            if (Vector3.SqrMagnitude(transform.position - posVec) > 50f)
            {
                transform.position = Vector3.MoveTowards(transform.position, posVec, 0.8f);

                foreach (Transform t in gangTransforms)
                {
                    if (t != transform)
                    {
                        t.LookAt(lookPos);
                        
                    }
                }

            }

        }
    }


    //hepsini kapat bazılarını ac gravity icin

    //ladderlength is the length of the ladder which is decided by the member count to create the ladder
    //diffBtwLadderMembers is the how much should ladder increase in y direction which each step, difference between each one of the ladder members
    //as the firstMemberOfLadder we should send the first collided member of the gang to start creating ladder at its position
    public IEnumerator CreateLadder(int ladderLength, int diffBtwLadderMembers, Transform firstMemberOfLadder, Transform lookPosition)
    {
        DataScript.inputLock = true;
        Vector3 memberPosInLadder;
        Vector3 ladderStartPos = firstMemberOfLadder.position;
        ladderStartPos.z -= 2f;     //change it do dynamic
        memberPosInLadder = firstMemberOfLadder.position;

        foreach (Animator gangMemberAnim in gangAnimators)
        {
            gangMemberAnim.SetBool("isWalking", false);
        }

        //firstMemberOfLadder.gameObject.GetComponent<Animator>().SetBool("isClimbing", true);
        //firstMemberOfLadder.gameObject.GetComponent<Animator>().SetBool("isClimbFinished", true);

        //firstMemberOfLadder.parent = null;
        SetGangList();

        //for creating the ladder
        for (int i = 0; i < ladderLength; i++)
        {
            if (i < gangTransforms.Count)
            {
                
                StartCoroutine(gangTransforms[i].gameObject.GetComponent<MemberActions>().CreateLadder(true, ladderStartPos, memberPosInLadder, lookPosition));
                memberPosInLadder.y = memberPosInLadder.y + diffBtwLadderMembers;
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        //yield return new WaitForSecondsRealtime(2f);        //member actions bitince done tarzı bisey gönderip yapabiliriz. biri bitmeden diğerine baslamasın diye
        //SetGangList();
        memberPosInLadder.y += diffBtwLadderMembers;

        //for sending rest of the gang to the top of the ladder
        if(ladderLength < gangTransforms.Count)
        {
            for (int i = ladderLength; i < gangTransforms.Count; i++)
            {
                StartCoroutine(gangTransforms[i].gameObject.GetComponent<MemberActions>().CreateLadder(false, ladderStartPos, memberPosInLadder, lookPosition));
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
       
        yield return new WaitForSecondsRealtime(3f);        //member actions bitince done tarzı bisey gönderip yapabiliriz. biri bitmeden diğerine baslamasın diye
        DataScript.inputLock = false;
        SetGangList();
        DataScript.memberCollisionLock = false;
        DataScript.isGravityOpen = true;
    }

    public IEnumerator CreateBridge(int bridgeLength, int diffBtwBridgeMembers, Transform firstMemberOfBridge, Transform lookPosition)
    {
        DataScript.inputLock = true;
        Vector3 memberPosInBridge;
        Vector3 bridgeStartPos = firstMemberOfBridge.position;
        bridgeStartPos.y += 2f;
        memberPosInBridge = bridgeStartPos;
        firstMemberOfBridge.position = bridgeStartPos;
        bridgeStartPos.z -= 2f;     //change it do dynamic
        

        foreach (Animator gangMemberAnim in gangAnimators)
        {
            gangMemberAnim.SetBool("isWalking", false);
        }

        
        Vector3 newLookPos = firstMemberOfBridge.transform.position;
        newLookPos.y -= 5f;
        firstMemberOfBridge.LookAt(newLookPos);
        firstMemberOfBridge.gameObject.GetComponent<Animator>().SetBool("isClimbing", true);
        firstMemberOfBridge.gameObject.GetComponent<Animator>().SetBool("isClimbFinished", true);

        firstMemberOfBridge.parent = null;
        SetGangList();

        //for creating the bridge
        for (int i = 0; i < bridgeLength - 1; i++)
        {
            if (i < gangTransforms.Count)
            {
                memberPosInBridge.z = memberPosInBridge.z + diffBtwBridgeMembers;
                StartCoroutine(gangTransforms[i].gameObject.GetComponent<MemberActions>().CreateBridge(true, bridgeStartPos, memberPosInBridge, lookPosition));
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        //yield return new WaitForSecondsRealtime(2f);        //member actions bitince done tarzı bisey gönderip yapabiliriz. biri bitmeden diğerine baslamasın diye
        //SetGangList();
        memberPosInBridge.z += diffBtwBridgeMembers;

        //for sending rest of the gang to the top of the ladder
        if (bridgeLength - 1 < gangTransforms.Count)
        {
            for (int i = bridgeLength - 1; i < gangTransforms.Count; i++)
            {
                StartCoroutine(gangTransforms[i].gameObject.GetComponent<MemberActions>().CreateBridge(false, bridgeStartPos, memberPosInBridge, lookPosition));
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        yield return new WaitForSecondsRealtime(3f);        //member actions bitince done tarzı bisey gönderip yapabiliriz. biri bitmeden diğerine baslamasın diye
        DataScript.inputLock = false;
        SetGangList();
        DataScript.memberCollisionLock = false;
        DataScript.isGravityOpen = true;
    }

    public void SetGangList()
    {
        gangAnimators.Clear();
        gangTransforms.Clear();

        gangAnimators.AddRange(GetComponentsInChildren<Animator>());

        foreach (Transform firstDepthChildT in gameObject.transform)
        {
            gangTransforms.Add(firstDepthChildT);
        }

        if(gangTransforms != null)
        {
            cameraScript.objectFollowedByCam = gangTransforms[0].gameObject;
        }
        
    }
}
