using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;
using System;

public class GangMovementScript : MonoBehaviour
{
    void StopWalkingAnimation(MotherGang.Gang gang)
    {
        foreach (MotherGang.GangMember gangMember in gang.MovableMembers)
        {
            gangMember.animator.SetBool("isWalking", false);
        }
    }
    
    void StartWalkingAnimation(MotherGang.Gang gang)
    {
        foreach (MotherGang.GangMember gangMember in gang.MovableMembers)
        {
            gangMember.animator.SetBool("isWalking", true);
        }
    }

    Vector2 touchStartPos;
    Vector2 touchDelta;
    public void MoveTheGang(GeneralInput gInput, MotherGang.Gang gang)
    {
        if (gInput.phase == IPhase.Began)
        {
            touchStartPos = gInput.currentPosition;            

            StartWalkingAnimation(gang);

            DataManager.instance.currentGangState = MotherGang.GangState.Walking;

        }
        else if (gInput.phase == IPhase.Ended)
        {
            touchDelta = Vector2.zero;
            StopWalkingAnimation(gang);

            DataManager.instance.currentGangState = MotherGang.GangState.Idle;
        }
        else //if (gInput.phase == IPhase.Moved || gInput.phase == IPhase.Stationary)
        {
            touchDelta = (gInput.currentPosition - touchStartPos);
            touchStartPos = Vector2.MoveTowards(touchStartPos, gInput.currentPosition, Vector2.Distance(touchStartPos, gInput.currentPosition) / 50f);

            JoyStickMovement(gang.Base, touchDelta);

            foreach (MotherGang.GangMember mem in gang.MovableMembers)
            {
                mem.member.MoveTowards(gang.Base);
            }
        }
    }


    public void JoyStickMovement(Transform memberTransform ,Vector2 posDelta)
    {
        Vector3 posVec = memberTransform.position;
        posVec.x += posDelta.x;
        posVec.z += posDelta.y;
        memberTransform.position = Vector3.MoveTowards(memberTransform.position, posVec, 0.5f);

        Vector3 lookPos = posVec;
        lookPos.y = memberTransform.position.y;

        memberTransform.LookAt(lookPos);
    }



    //hepsini kapat bazılarını ac gravity icin

    //ladderlength is the length of the ladder which is decided by the member count to create the ladder
    //diffBtwLadderMembers is the how much should ladder increase in y direction which each step, difference between each one of the ladder members
    //as the firstMemberOfLadder we should send the first collided member of the gang to start creating ladder at its position
    public IEnumerator CreateLadder(MotherGang.Gang gang, int ladderLength,Transform obstacle)
    {
        //memberPosInLadder i ayarla. Pozisyonu baseHead olcak.
        Vector3 memberPosInLadder = gang.Base.GetChild(0).position;
        //memberlar arasi y mesafesi
        float yDistBtwMembers = gang.MovableMembers[0].transform.lossyScale.y * 3;

        Vector3 ladderStartPos = memberPosInLadder;
        ladderStartPos.y = gang.MovableMembers[0].transform.position.y;     //change it do dynamic

        StopWalkingAnimation(gang);

        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);

        //temp listeyi olustur ve orijinal ini sil.
        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        //Base in tirmanma sonunda nereye gelcegine karar ver. Sonra bi action ata ki merdiven bitince base yukari tasinsin
        float yPos = obstacle.transform.position.y + (obstacle.transform.localScale.y / 2f) + (gang.Base.transform.localScale.y);
        float zPos = obstacle.transform.position.z - (obstacle.transform.localScale.z / 2f) + (gang.Base.transform.localScale.z / 2f);

        Vector3 newBasePosition = new Vector3(memberPosInLadder.x, yPos, zPos);

        //Bu action son member merdiven oldugunda caigiriliyor (member.CreateLadder da null olarak gtmeyince sonunda cagiriliyor). Base i yukari tasiyor
        Action moveBaseUpwards = delegate () { SetGangBasePosition(gang.Base, newBasePosition); };

        //for creating the ladder
        for (int i = 0; i < ladderLength; i++)
        {
            if (i < tmpMovables.Count)
            {
                //sonuncu iteration sa bu corountine bitince base i yukari tasi
                if(i == ladderLength - 1)
                {
                    tmpMovables[i].member.CreateLadder(gang, true, ladderStartPos, memberPosInLadder, moveBaseUpwards);
                }
                else
                {
                    tmpMovables[i].member.CreateLadder(gang, true, ladderStartPos, memberPosInLadder);
                }

                
                tmpMovables.RemoveAt(i);
                
                memberPosInLadder.y = memberPosInLadder.y + yDistBtwMembers;
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f,0.2f));
            }
        }

        //base i en uste tasidiktan sonra movable objeleri dolandirmaya basla
        DataManager.instance.currentGangState = MotherGang.GangState.Idle;

        //for sending rest of the gang to the top of the ladder
        for (int i = 0; i < tmpMovables.Count; i++)
        {
            tmpMovables[i].member.CreateLadder(gang, false, ladderStartPos, memberPosInLadder);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
        }
    }

    public IEnumerator CreateBridge(MotherGang.Gang gang, int bridgeLength, Transform obstacle)
    {
        //memberPosInLadder i ayarla. Pozisyonu baseHead olcak.
        Vector3 memberPosInBridge = gang.Base.GetChild(0).position;
        memberPosInBridge.y = obstacle.position.y + (obstacle.localScale.y / 2f);
        //memberlar arasi y mesafesi
        float zDistBtwMembers = gang.MovableMembers[0].transform.lossyScale.y * 3;

        Vector3 bridgeStartPos = memberPosInBridge;
        bridgeStartPos.y = obstacle.position.y + (obstacle.localScale.y / 2f);

        StopWalkingAnimation(gang);

        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);
        //tmpMovables = gang.MovableMembers;

        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        //Base in kopru kurma sonunda nereye gelcegine karar ver. Sonra bi action ata ki kopru bitince base ileri tasinsin
        float zPos = obstacle.transform.position.z + (obstacle.transform.localScale.z / 2f) + (gang.Base.transform.localScale.z / 2f);
        Vector3 newBasePosition = new Vector3(gang.Base.transform.position.x, gang.Base.transform.position.y, zPos);

        //Bu action son member kopru oldugunda caigiriliyor (member.CreateBridge da null olarak gtmeyince sonunda cagiriliyor). Base i ileri (newBasePosition a) tasiyor
        Action moveBaseForward = delegate () { SetGangBasePosition(gang.Base, newBasePosition); };

        //for creating the bridge
        for (int i = 0; i < bridgeLength; i++)
        {
            if (i < tmpMovables.Count)
            {
                //sonuncu iteration sa bu corountine bitince base i yukari tasi
                if (i == bridgeLength - 1)
                {
                    tmpMovables[i].member.CreateBridge(gang, true, bridgeStartPos, memberPosInBridge, moveBaseForward);
                }
                else
                {
                    tmpMovables[i].member.CreateBridge(gang, true, bridgeStartPos, memberPosInBridge);
                }

                tmpMovables.RemoveAt(i);

                memberPosInBridge.z = memberPosInBridge.z + zDistBtwMembers;
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
            }
        }

        //base i ileri tasidiktan sonra movable objeleri dolandirmaya basla
        DataManager.instance.currentGangState = MotherGang.GangState.Idle;

        //for sending rest of the gang to pass the bridge
        if (bridgeLength < tmpMovables.Count)
        {
            for (int i = 0; i < tmpMovables.Count; i++)
            {
                tmpMovables[i].member.CreateBridge(gang, false, bridgeStartPos, memberPosInBridge);
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
            }
        }
    }

    void SetGangBasePosition(Transform gangBase, Vector3 pos)
    {
        gangBase.transform.position = pos;
    }
}








///OLD MOVE /////
/// 
/// 
        /*
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
     */
