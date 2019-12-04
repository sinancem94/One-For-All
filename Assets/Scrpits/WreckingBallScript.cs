using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBallScript : MonoBehaviour
{

    private Quaternion wreckingBallRotation;

    private bool isTurningRight;

    void Start()
    {
        StartCoroutine(SetTurningPos());
        StartCoroutine(SetWreckingBallPos());
        isTurningRight = true;
    }

    IEnumerator SetTurningPos()
    {
        while (true)
        {
            if (isTurningRight)
                isTurningRight = false;
            else
                isTurningRight = true;
            yield return new WaitForSecondsRealtime(1f);
        }
      
    }

    IEnumerator SetWreckingBallPos()
    {
        while (true)
        {
            if (isTurningRight)
            {
                transform.Rotate(3f, 0, 0);
            }
            else
            {
                transform.Rotate(-3f, 0, 0);
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
}
