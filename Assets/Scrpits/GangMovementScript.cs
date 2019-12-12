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
                mem.member.SetNewPosition(gang.Base);
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

    public void LockAllMembers(MotherGang.Gang gang)
    {
        foreach (MotherGang.GangMember gangMember in gang.MovableMembers)
        {
            gangMember.member.memberLock = true;
        }
    }

    public IEnumerator PassLadder(MotherGang.Gang gang, Obstacle ladder, Vector2 directionVec)
    {
        Vector3 ladderStartPosition = ladder.passStartPoisition;

        LockAllMembers(gang);
        StopWalkingAnimation(gang);

        int direction = (directionVec.y == 1) ? 1 : -1;
        //Base in tirmanma sonunda nereye gelcegine karar ver. Sonra bi action ata ki merdiven bitince base yukari tasinsin
        float yPos = ladder.transform.position.y + (ladder.transform.localScale.y / 2f * direction) + (gang.Base.transform.localScale.y);
        float zPos = ladder.transform.position.z - (ladder.transform.localScale.z / 2f) + (gang.Base.transform.localScale.z / 2f);

        Vector3 newBasePosition = new Vector3(ladderStartPosition.x, yPos, zPos);
        
        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);

        //temp listeyi olustur ve orijinal ini sil.
        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        //Bu action son member merdiven oldugunda caigiriliyor (member.CreateLadder da null olarak gtmeyince sonunda cagiriliyor). Base i yukari tasiyor
        Action moveBase = delegate () { SetGangBasePosition(gang.Base, newBasePosition);
            //base i ileri tasidiktan sonra movable objeleri dolandirmaya basla
            DataManager.instance.currentGangState = MotherGang.GangState.Idle;
        };

        //for sending rest of the gang to the top of the ladder
        for (int i = 0; i < tmpMovables.Count; i++)
        {
            if(i == 0)
            {
                tmpMovables[i].member.ClimbLadder(gang, ladderStartPosition, newBasePosition, moveBase);
            }
            else
            {
                tmpMovables[i].member.ClimbLadder(gang, ladderStartPosition, newBasePosition);
            }
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
        }

    }

    public IEnumerator PassBridge(MotherGang.Gang gang, Obstacle bridge, Vector2 directionVec)
    {
        Vector3 bridgeStartPosition = bridge.passStartPoisition;

        LockAllMembers(gang);
        StopWalkingAnimation(gang);

        int direction = (directionVec.y == 1) ? 1 : -1;
      
        //Base in kopru kurma sonunda nereye gelcegine karar ver. Sonra bi action ata ki kopru bitince base ileri tasinsin
        float zPos = bridge.transform.position.z + (bridge.transform.localScale.z / 2f * direction) + (gang.Base.transform.localScale.z / 2f);
        Vector3 newBasePosition = new Vector3(gang.Base.transform.position.x, gang.Base.transform.position.y, zPos);

        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);

        //temp listeyi olustur ve orijinal ini sil.
        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        //Bu action son member merdiven oldugunda caigiriliyor (member.CreateLadder da null olarak gtmeyince sonunda cagiriliyor). Base i yukari tasiyor
        Action moveBase = delegate () {
            SetGangBasePosition(gang.Base, newBasePosition);
            //base i ileri tasidiktan sonra movable objeleri dolandirmaya basla
            DataManager.instance.currentGangState = MotherGang.GangState.Idle;
        };

        //for sending rest of the gang to the top of the ladder
        for (int i = 0; i < tmpMovables.Count; i++)
        {
            if (i == 0)
            {
                tmpMovables[i].member.PassBridge(gang, bridgeStartPosition, newBasePosition, moveBase);
            }
            else
            {
                tmpMovables[i].member.PassBridge(gang, bridgeStartPosition, newBasePosition);//(gang, false, ladderStartPos, memberPosInLadder);
            }
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
        }
    }

    
    /// <summary>
    /// Called from motherGang when a collision with an ladderObstacle occurs
    /// </summary>
    /// <param name="gang"></param>
    /// gang object that holds movable gang. 
    /// 
    /// <param name="ladderLength"></param>
    /// ladderlength is the length of the ladder which is decided by the member count to create the ladder
    /// 
    /// <param name="ladder"></param>
    /// ladderObstacle that gang collides that collided 
    /// 
    /// <param name="direction"></param>
    /// Direction of movement. Should be Vector2.up or Vector2.down
    /// Changes yDistBtwMembers if up yDistBtwMembers > 0 , if down yDistBtwMembers < 0
    /// 
    /// <returns></returns>

    public IEnumerator CreateLadder(MotherGang.Gang gang, int ladderLength,Obstacle ladder,Vector2 directionVec)
    {
        //memberPosInLadder i ayarla. Pozisyonu baseHead olcak.
        Vector3 memberPosInLadder = gang.baseHead.position;

        //Directiona gore -1 ya da 1
        int direction = (directionVec.y == 1) ? 1 : -1;
        //memberlar arasi y mesafesi. direction a gore yukari ya da asagi dogru hareket et
        float yDistBtwMembers = gang.MovableMembers[0].transform.lossyScale.y * 3 * direction;

        Vector3 ladderStartPos = memberPosInLadder;
        ladderStartPos.y = gang.MovableMembers[0].transform.position.y;     //change it do dynamic

        LockAllMembers(gang);
        StopWalkingAnimation(gang);

        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);

        //merdiveni olusturcak memberlar
        List<MotherGang.GangMember> ladderMembers = new List<MotherGang.GangMember>(ladderLength);

        //temp listeyi olustur ve orijinal ini sil.
        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        //Base in tirmanma sonunda nereye gelcegine karar ver. Sonra bi action ata ki merdiven bitince base yukari tasinsin
        float yPos = ladder.transform.position.y + (ladder.transform.localScale.y / 2f * direction) + (gang.Base.transform.localScale.y);
        float zPos = ladder.transform.position.z - (ladder.transform.localScale.z / 2f) + (gang.Base.transform.localScale.z / 2f);

        Vector3 newBasePosition = new Vector3(memberPosInLadder.x, yPos, zPos);

        //Bu action son member merdiven oldugunda caigiriliyor (member.CreateLadder da null olarak gtmeyince sonunda cagiriliyor). Base i yukari tasiyor
        Action moveBase = delegate () { SetGangBasePosition(gang.Base, newBasePosition); 
            //base i en uste tasidiktan sonra movable objeleri dolandirmaya basla
            DataManager.instance.currentGangState = MotherGang.GangState.Idle;
        };

        //for creating the ladder
        for (int i = 0; i < ladderLength; i++)
        {
            if (i < tmpMovables.Count)
            {
                //sonuncu iteration sa bu corountine bitince base i yukari tasi
                if(i == ladderLength - 1)
                {
                    tmpMovables[i].member.CreateLadder(gang, ladderStartPos, memberPosInLadder, moveBase);
                }
                else
                {
                    tmpMovables[i].member.CreateLadder(gang, ladderStartPos, memberPosInLadder);
                }

                ladderMembers.Add(tmpMovables[i]);
                tmpMovables.RemoveAt(i);


                memberPosInLadder.y = memberPosInLadder.y + yDistBtwMembers;
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f,0.2f));
            }
        }

        ladder.CreateObstacleMembers(ladderMembers);
        

        //for sending rest of the gang to the top of the ladder
        for (int i = 0; i < tmpMovables.Count; i++)
        {
            tmpMovables[i].member.ClimbLadder(gang, ladderStartPos, memberPosInLadder);//(gang, false, ladderStartPos, memberPosInLadder);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
        }
    }

    /// <summary>
    /// Called from motherGang when a collision with an bridgeObstacle occurs
    /// </summary>
    /// <param name="gang"></param>
    /// bridgeLength is the length of the ladder which is decided by the member count to create the ladder
    /// 
    /// <param name="bridgeLength"></param>
    /// bridgeObstacle that gang collides that collided 
    /// 
    /// <param name="bridge"></param>
    /// bridge that gang collided with
    /// 
    /// <param name="direction"></param>
    /// Direction of movement. Should be Vector2.up or Vector2.down
    /// Changes yDistBtwMembers if up yDistBtwMembers > 0 , if down yDistBtwMembers < 0
    /// 
    /// <returns></returns>
    public IEnumerator CreateBridge(MotherGang.Gang gang, int bridgeLength, Obstacle bridge, Vector2 directionVec)
    {
        //memberPosInLadder i ayarla. Pozisyonu baseHead olcak.
        Vector3 memberPosInBridge = gang.baseHead.position;
        memberPosInBridge.y = bridge.transform.position.y + (bridge.transform.localScale.y / 2f);

        //Directiona gore -1 ya da 1
        int direction = (directionVec.y == 1) ? 1 : -1;
        //memberlar arasi y mesafesi. direction a gore yukari ya da asagi dogru hareket et
        float zDistBtwMembers = gang.MovableMembers[0].transform.lossyScale.y * 3 * direction;

        Vector3 bridgeStartPos = memberPosInBridge;
        bridgeStartPos.y = bridge.transform.position.y + (bridge.transform.localScale.y / 2f);

        LockAllMembers(gang);
        StopWalkingAnimation(gang);

        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);

        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        //Base in kopru kurma sonunda nereye gelcegine karar ver. Sonra bi action ata ki kopru bitince base ileri tasinsin
        float zPos = bridge.transform.position.z + (bridge.transform.localScale.z / 2f * direction) + (gang.Base.transform.localScale.z / 2f);
        Vector3 newBasePosition = new Vector3(gang.Base.transform.position.x, gang.Base.transform.position.y, zPos);

        //Bu action son member kopru oldugunda caigiriliyor (member.CreateBridge da null olarak gtmeyince sonunda cagiriliyor). Base i ileri (newBasePosition a) tasiyor
        Action moveBaseForward = delegate () { SetGangBasePosition(gang.Base, newBasePosition);
            //base i ileri tasidiktan sonra movable objeleri dolandirmaya basla
            DataManager.instance.currentGangState = MotherGang.GangState.Idle;
        };

        List<MotherGang.GangMember> ladderMembers = new List<MotherGang.GangMember>(bridgeLength);


        //for creating the bridge
        for (int i = 0; i < bridgeLength; i++)
        {
            if (i < tmpMovables.Count)
            {
                //sonuncu iteration sa bu corountine bitince base i yukari tasi
                if (i == bridgeLength - 1)
                {
                    tmpMovables[i].member.CreateBridge(gang, bridgeStartPos, memberPosInBridge, moveBaseForward);
                }
                else
                {
                    tmpMovables[i].member.CreateBridge(gang, bridgeStartPos, memberPosInBridge);
                }

                ladderMembers.Add(tmpMovables[i]);
                tmpMovables.RemoveAt(i);

                memberPosInBridge.z = memberPosInBridge.z + zDistBtwMembers;
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
            }
        }

        bridge.CreateObstacleMembers(ladderMembers);

       

        //for sending rest of the gang to pass the bridge
        if (bridgeLength < tmpMovables.Count)
        {
            for (int i = 0; i < tmpMovables.Count; i++)
            {
                tmpMovables[i].member.PassBridge(gang, bridgeStartPos, memberPosInBridge);//(gang, false, bridgeStartPos, memberPosInBridge);
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
