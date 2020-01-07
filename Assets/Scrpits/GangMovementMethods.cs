using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GangMovementMethods 
{
    public Coroutine gangEvent;

   /* Func<Vector3> moveBase = new Action<Vector3>( (Vector3 newBasePosition) =>
    {
        SetGangBasePosition(DataManager.instance.GetGang().Base, newBasePosition);
        //base i ileri tasidiktan sonra movable objeleri dolandirmaya basla
        DataManager.instance.currentGangState = DataManager.GangState.Idle;
    });*/

    public void LockAllMembers(MotherGang.Gang gang)
    {
        foreach (MotherGang.GangMember gangMember in gang.MovableMembers)
        {
            gangMember.member.SetLock(true);
        }
    }


    public void JoyStickMovement(MotherGang.Gang gang ,Vector2 posDelta)
    {
        Vector3 inputDelta = gang.Base.position;
        inputDelta.x += posDelta.x;
        inputDelta.z += posDelta.y;
        //Debug.Log(inputDelta.normalized);
       // inputDelta = Camera.main.ScreenToWorldPoint(inputDelta);

       //inputDelta.x = Mathf.Clamp(inputDelta.x, mostLeft, mostRight);
        //inputDelta.y = gang.Base.position.y;
       // inputDelta.z = gang.Base.position.z;

        Vector3 newPosition = Vector3.MoveTowards(gang.Base.transform.position, inputDelta, gang.speed);// posVec - gang.Base.transform.position;

        //Vector3 forceApplier = new Vector3(inputDelta.x, 0f, inputDelta.z);

        //Debug.Log(forceApplier.normalized);

        gang.Base.LookAt(inputDelta);

        gang.Rb.MovePosition(newPosition);

    }


    public void PassObstacle(MonoBehaviour owner, Obstacle obstacle)
    {
        if (gangEvent == null)
        {
            Action StopMe = delegate () {
                if(gangEvent != null)
                {
                    owner.StopCoroutine(gangEvent);
                    gangEvent = null;
                } 
            };

            gangEvent = owner.StartCoroutine(PassObjectCorountine(DataManager.instance.GetGang(), obstacle, StopMe));
        }
    }

    public void CreateObstaclePass(MonoBehaviour owner, int passLength, Obstacle obstacle)
    {
        if(gangEvent == null)
        {
            Action StopMe = delegate () {
                if (gangEvent != null)
                {
                    owner.StopCoroutine(gangEvent);
                    gangEvent = null;
                }
            };


            gangEvent = owner.StartCoroutine(CreateObstaclePassCorountine(DataManager.instance.GetGang(), passLength, obstacle,StopMe));
        }
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
    /// for ladder Changes yDistBtwMembers if up yDistBtwMembers > 0 , if down yDistBtwMembers < 0
    /// for bridge changes zD
    /// <returns></returns>
    IEnumerator CreateObstaclePassCorountine(MotherGang.Gang gang, int passLength, Obstacle obstacle,Action stopMe)
    {
        LockAllMembers(gang);

        //memberPosInPass i ayarla. Pozisyonu baseHead olcak.
        Vector3 memberPosInPass = gang.baseHead.position;
        memberPosInPass.y = gang.MovableMembers[0].transform.position.y;
        memberPosInPass.z = obstacle.transform.position.z - (obstacle.transform.localScale.z / 2f);

        Vector3 passStartPosition = memberPosInPass;

        //Directiona gore -1 ya da 1
        int direction = 1;// (directionVec.y == 1) ? 1 : -1;

        //memberlar arasi y mesafesi. direction a gore yukari ya da asagi dogru hareket et
        float distBtwUsedPassMembers = gang.MovableMembers[0].transform.lossyScale.y * 3 * direction;

        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);
        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        //Gecisten sonra gangBase i nereye gitcek ona karar ver
        Vector3 newBasePosition = gang.Base.transform.position;
        float yPos = newBasePosition.y;
        float zPos = obstacle.transform.position.z + (obstacle.transform.lossyScale.z / 2f * direction) + (gang.Base.transform.localScale.z / 2f * direction) + 1f;
        //eger gecis turu merdiven ise gang in y pozisyonunu da degistir
        if (obstacle.ObstacleType == Obstacle.Type.Ladder)
        {
            yPos = obstacle.transform.position.y + (obstacle.transform.lossyScale.y / 2f * direction) + (gang.Base.transform.localScale.y);
        }
        newBasePosition = new Vector3(newBasePosition.x, yPos, zPos);

        //Bu action son member kopru oldugunda caigiriliyor (member.CreateBridge da null olarak gtmeyince sonunda cagiriliyor). Base i ileri (newBasePosition a) tasiyor
        Action moveBase = delegate () {
            SetGangStateIdleAndPosition(newBasePosition);
            //base i ileri tasidiktan sonra movable objeleri dolandirmaya basla
        };

        List<MotherGang.GangMember> obstacleMembers = new List<MotherGang.GangMember>(passLength);

        // player could not reach the top of pass. Set createdPassLength so there will be no segmentation. 
        int createdPassLength = 0;


        //for creating the pass. Set pass members and send them 
        for (int i = 0; i < passLength; i++)
        {
            if (i < tmpMovables.Count)
            {
                //sonuncu iteration sa bu corountine bitince base i yukari tasi
                if (i == passLength - 1)
                {
                    tmpMovables[i].member.CreateObstaclePass(gang, tmpMovables[i], obstacle, passStartPosition, memberPosInPass,moveBase);
                }
                else
                {
                    tmpMovables[i].member.CreateObstaclePass(gang, tmpMovables[i], obstacle, passStartPosition, memberPosInPass);
                }

                obstacleMembers.Add(tmpMovables[i]);
                createdPassLength++;

                //pass yaratilan objeye gore memberPosInPass i degistir
                if (obstacle.ObstacleType == Obstacle.Type.Bridge)
                    memberPosInPass.z = memberPosInPass.z + distBtwUsedPassMembers;
                else if(obstacle.ObstacleType == Obstacle.Type.Ladder)
                    memberPosInPass.y = memberPosInPass.y + distBtwUsedPassMembers;

                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
            }
        }

        Vector3 passEndPosition = memberPosInPass;

        tmpMovables.RemoveRange(0, createdPassLength);
        obstacle.SetAsPassableObstacle(obstacleMembers,passStartPosition,passEndPosition);

        for (int i = 0; i < tmpMovables.Count; i++)
        {
            tmpMovables[i].member.PassObstacle(gang, tmpMovables[i], obstacle);//(gang, false, bridgeStartPos, memberPosInBridge);
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
        }

        //gang.AllGang.RemoveRange(0, createdPassLength);

        stopMe();
    }

    /// <summary>
    /// if obstacle pass is created and gang collided with it pass the gang thourgh pass
    /// </summary>
    /// <param name="gang"></param>
    /// <param name="obstacle"></param>
    /// <param name="directionVec"></param>
    /// <returns></returns>
    IEnumerator PassObjectCorountine(MotherGang.Gang gang, Obstacle obstacle,Action stopMe)
    {
        LockAllMembers(gang);

        //Directiona gore -1 ya da 1
        int direction = obstacle.SetPassStartAndEndPositions();//(directionVec.y == 1) ? 1 : -1;

        Vector3 newBasePosition = obstacle.passEndPosition;
        newBasePosition.y = newBasePosition.y + (obstacle.transform.localScale.y / 2f);
        newBasePosition.z = obstacle.transform.position.z + (obstacle.transform.localScale.z / 2f * direction) + (gang.Base.transform.localScale.z / 2f * direction) + 1f;

        //copy movable members to temp and delete movables. 
        List<MotherGang.GangMember> tmpMovables = new List<MotherGang.GangMember>(gang.MovableMembers.Count);
        tmpMovables.AddRange(gang.MovableMembers);
        gang.MovableMembers.Clear();

        //Bu action son member merdiven oldugunda caigiriliyor (member.CreateLadder da null olarak gtmeyince sonunda cagiriliyor). Base i yukari tasiyor
        Action moveBase = delegate () {
            SetGangStateIdleAndPosition(newBasePosition);
        };

        //for sending gang to the top of the object
        for (int i = 0; i < tmpMovables.Count; i++)
        {
            if (i == 0)
            {
                tmpMovables[i].member.PassObstacle(gang, tmpMovables[i], obstacle, moveBase);
            }
            else
            {
                tmpMovables[i].member.PassObstacle(gang, tmpMovables[i], obstacle);
            }
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.05f, 0.2f));
        }

        stopMe();
    }

    static void SetGangStateIdleAndPosition(Vector3 pos)
    {
        DataManager.instance.GetGang().Base.position = pos;
        //base i ileri tasidiktan sonra movable objeleri dolandirmaya basla
        DataManager.instance.currentGangState = DataManager.GangState.Idle;
    }
}

///merhaba, bu oyunu 37684673756 tc kimlik numarali melis idil koker yapmis olup Sinan cem cayli isimli irz dusmani oyunumu calmistir. 
///calarken musa karakelleoglu denen pezevengin burnundan yardim almislardir. 
///huroglu denen arkadas ise her zamanki gibi hicbirimizi sasirtmayarak hicbir sey yapmamis ve sadece oyunumun calinmasi fikrine heyecanlanarak dahil olmustur.
///tesekkurler.
///

