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

    public void LockAllMembers(MotherGang.Gang gang)
    {
        foreach (MotherGang.GangMember gangMember in gang.MovableMembers)
        {
            gangMember.member.memberLock = true;
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

            DataManager.instance.currentGangState = DataManager.GangState.Walking;

        }
        else if (gInput.phase == IPhase.Ended)
        {
            touchDelta = Vector2.zero;
            StopWalkingAnimation(gang);

            DataManager.instance.currentGangState = DataManager.GangState.Idle;
        }
        else //if (gInput.phase == IPhase.Moved || gInput.phase == IPhase.Stationary)
        {
            touchDelta = (gInput.currentPosition - touchStartPos);
            touchStartPos = Vector2.MoveTowards(touchStartPos, gInput.currentPosition, Vector2.Distance(touchStartPos, gInput.currentPosition) / 50f);

            JoyStickMovement(gang.Base, touchDelta);
        }
    }


    public void JoyStickMovement(Transform movedObject ,Vector2 posDelta)
    {
        Vector3 posVec = movedObject.position;
        posVec.x += posDelta.x;
        posVec.z += posDelta.y;

        movedObject.LookAt(posVec);

        movedObject.position = Vector3.MoveTowards(movedObject.position, posVec, 0.5f);
    }

   

    /// <summary>
    /// Called from motherGang when a collision with an bridgeObstacle occurs
    /// </summary>
    /// <param name="gang"></param>
    /// passLength is the length of the ladder which is decided by the member count to create the object
    /// 
    /// <param name="bridgeLength"></param>
    /// obstacle object that gang collides that collided 
    /// 
    /// <param name="direction"></param>
    /// Direction of movement. Should be Vector2.up or Vector2.down
    /// Changes yDistBtwMembers if up yDistBtwMembers > 0 , if down yDistBtwMembers < 0
    /// 
    /// <returns></returns>
    public IEnumerator CreateObstaclePass(MotherGang.Gang gang, int passLength, Obstacle obstacle, Vector2 directionVec)
    {
        LockAllMembers(gang);
        StopWalkingAnimation(gang);

        //memberPosInPass i ayarla. Pozisyonu baseHead olcak.
        Vector3 memberPosInPass = gang.baseHead.position;
        memberPosInPass.y = gang.MovableMembers[0].transform.position.y;

        Vector3 passStartPosition = memberPosInPass;

        //Directiona gore -1 ya da 1
        int direction = (directionVec.y == 1) ? 1 : -1;

        //memberlar arasi y mesafesi. direction a gore yukari ya da asagi dogru hareket et
        float distBtwUsedPassMembers = gang.MovableMembers[0].transform.lossyScale.y * 3 * direction;

        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);
        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        Vector3 newBasePosition = gang.Base.transform.position;

        if(obstacle.ObstacleType == Obstacle.Type.Bridge)
        {
            //Base in kopru kurma sonunda nereye gelcegine karar ver. Sonra bi action ata ki kopru bitince base ileri tasinsin
            float zPos = obstacle.transform.position.z + (obstacle.transform.localScale.z / 2f * direction) + (gang.Base.transform.localScale.z / 2f * direction);
            newBasePosition = new Vector3(gang.Base.transform.position.x, gang.Base.transform.position.y, zPos);
        }
        else if(obstacle.ObstacleType == Obstacle.Type.Ladder)
        {
            //Base in tirmanma sonunda nereye gelcegine karar ver. Sonra bi action ata ki merdiven bitince base yukari tasinsin
            float yPos = obstacle.transform.position.y + (obstacle.transform.localScale.y / 2f * direction) + (gang.Base.transform.localScale.y );
            float zPos = obstacle.transform.position.z - (obstacle.transform.localScale.z / 2f) + (gang.Base.transform.localScale.z / 2f);
            newBasePosition = new Vector3(gang.Base.transform.position.x, yPos, zPos);
        }

        //Bu action son member kopru oldugunda caigiriliyor (member.CreateBridge da null olarak gtmeyince sonunda cagiriliyor). Base i ileri (newBasePosition a) tasiyor
        Action moveBase = delegate () {
            SetGangBasePosition(gang.Base, newBasePosition);
            //base i ileri tasidiktan sonra movable objeleri dolandirmaya basla
            DataManager.instance.currentGangState = DataManager.GangState.Idle;
        };

        List<MotherGang.GangMember> obstacleMembers = new List<MotherGang.GangMember>(passLength);

        //for creating the bridge
        for (int i = 0; i < passLength; i++)
        {
            if (i < tmpMovables.Count)
            {
                //sonuncu iteration sa bu corountine bitince base i yukari tasi
                if (i == passLength - 1)
                {
                    tmpMovables[i].member.CreateObstaclePass(gang, obstacle, passStartPosition, memberPosInPass, moveBase);
                }
                else
                {
                    tmpMovables[i].member.CreateObstaclePass(gang, obstacle, passStartPosition, memberPosInPass);
                }

                obstacleMembers.Add(tmpMovables[i]);

                //pass yaratilan objeye gore memberPosInPass i degistir
                if(obstacle.ObstacleType == Obstacle.Type.Bridge)
                    memberPosInPass.z = memberPosInPass.z + distBtwUsedPassMembers;
                else if(obstacle.ObstacleType == Obstacle.Type.Ladder)
                    memberPosInPass.y = memberPosInPass.y + distBtwUsedPassMembers;

                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
            }
        }

        Vector3 passEndPosition = memberPosInPass;

        tmpMovables.RemoveRange(0, passLength);
        obstacle.CreateObstacleMembers(obstacleMembers,passStartPosition,passEndPosition);

        for (int i = 0; i < tmpMovables.Count; i++)
        {
            tmpMovables[i].member.PassObstacle(gang, obstacle);//(gang, false, bridgeStartPos, memberPosInBridge);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
        }

        gang.AllGang.RemoveRange(0, passLength);
    }
    /// <summary>
    /// if obstacle pass is created and gang collided with it pass the gang thourgh pass
    /// </summary>
    /// <param name="gang"></param>
    /// <param name="obstacle"></param>
    /// <param name="directionVec"></param>
    /// <returns></returns>
    public IEnumerator PassObject(MotherGang.Gang gang, Obstacle obstacle, Vector2 directionVec)
    {
        LockAllMembers(gang);
        StopWalkingAnimation(gang);

        //Directiona gore -1 ya da 1
        int direction = (directionVec.y == 1) ? 1 : -1;

        //calculate starting position and ending position if obstacle is a bridge look at z positions
        if (obstacle.ObstacleType == Obstacle.Type.Bridge)
        {
            if(obstacle.passStartMember.z > obstacle.passEndMember.z && direction == 1)
            {
                Vector3 tmpPos = obstacle.passStartMember;
                obstacle.passStartMember = obstacle.passEndMember;
                obstacle.passEndMember = tmpPos;
            }
            else if(obstacle.passStartMember.z < obstacle.passEndMember.z && direction != 1)
            {
                Vector3 tmpPos = obstacle.passStartMember;
                obstacle.passStartMember = obstacle.passEndMember;
                obstacle.passEndMember = tmpPos;
            }
        }
        //calculate starting position and ending position if obstacle is a ladder look at y positions
        else if (obstacle.ObstacleType == Obstacle.Type.Ladder)
        {
            if (obstacle.passStartMember.y > obstacle.passEndMember.y && direction == 1)
            {
                Vector3 tmpPos = obstacle.passStartMember;
                obstacle.passStartMember = obstacle.passEndMember;
                obstacle.passEndMember = tmpPos;
            }
            else if (obstacle.passStartMember.y < obstacle.passEndMember.y && direction != 1)
            {
                Vector3 tmpPos = obstacle.passStartMember;
                obstacle.passStartMember = obstacle.passEndMember;
                obstacle.passEndMember = tmpPos;
            }
        }

        Vector3 newBasePosition = obstacle.passEndMember;
        newBasePosition.z = newBasePosition.z + (obstacle.transform.localScale.z / 3f * direction);

        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);
        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        //Bu action son member merdiven oldugunda caigiriliyor (member.CreateLadder da null olarak gtmeyince sonunda cagiriliyor). Base i yukari tasiyor
        Action moveBase = delegate () {
            SetGangBasePosition(gang.Base, newBasePosition);
            //base i ileri tasidiktan sonra movable objeleri dolandirmaya basla
            DataManager.instance.currentGangState = DataManager.GangState.Idle;
        };

        //for sending gang to the top of the object
        for (int i = 0; i < tmpMovables.Count; i++)
        {
            if (i == 0)
            {
                tmpMovables[i].member.PassObstacle(gang, obstacle, moveBase);
            }
            else
            {
                tmpMovables[i].member.PassObstacle(gang, obstacle);
            }
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
        }

    }

    void SetGangBasePosition(Transform gangBase, Vector3 pos)
    {
        gangBase.transform.position = pos;
    }
}

///merhaba, bu oyunu 37684673756 tc kimlik numarali melis idil koker yapmis olup Sinan cem cayli isimli irz dusmani oyunumu calmistir. 
///calarken musa karakelleoglu denen pezevengin burnundan yardim almislardir. 
///huroglu denen arkadas ise her zamanki gibi hicbirimizi sasirtmayarak hicbir sey yapmamis ve sadece oyunumun calinmasi fikrine heyecanlanarak dahil olmustur.
///tesekkurler.
///






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
