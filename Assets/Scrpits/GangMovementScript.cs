using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtmostInput;

public class GangMovementScript : MonoBehaviour
{
    private List<Animator> gangAnimators;
    private List<Transform> gangTransforms;
    Vector2 initialPos;

    InputX inputX;

    private void Start()
    {
        DataScript.inputLock = false;
        inputX = new InputX();
        initialPos = new Vector2();
        gangAnimators = new List<Animator>();
        gangTransforms = new List<Transform>();

        SetGangList();


    }

    void Update()
    {
        if (inputX.IsInput() && !DataScript.inputLock)
        {
            GeneralInput gInput = inputX.GetInput(0);
            MoveTheGang(gInput);
        }

        else
        {
            foreach (Animator gangMemberAnim in gangAnimators)
            {
                gangMemberAnim.SetBool("isWalking", false);
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
            transform.position = Vector3.MoveTowards(transform.position, posVec, 0.8f);

            foreach (Transform t in gangTransforms)
            {
                if (t != transform)
                {
                    t.LookAt(posVec);
                }
            }
            
        }
    }

    public IEnumerator CreateLadder(int ladderLength, int diffBtwLadderMembers)
    {
        DataScript.inputLock = true;
        Vector3 ladderPos;
        ladderPos = new Vector3(0, 3f, 0);

        for (int i = 0; i < ladderLength; i++)
        {

            while (Vector3.SqrMagnitude(gangTransforms[i].position - ladderPos) > 0.5f)
            {
                gangTransforms[i].position = Vector3.MoveTowards(gangTransforms[i].position, ladderPos, 1f);
                yield return new WaitForSecondsRealtime(0.1f);
            }
            ladderPos.y = ladderPos.y + diffBtwLadderMembers;
            gangTransforms[i].parent = null;
        }

        DataScript.inputLock = false;
        SetGangList();
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
    }
}
