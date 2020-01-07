using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBallScript : MonoBehaviour
{

    public float Startingtime;

    Vector3 to;
    Vector3 from;

    float radiance; 

    void Start()
    {
        to = new Vector3(-20f, 90f, 0f);
        from = new Vector3(-160f, 90f, 0f);

        radiance = Mathf.Abs(to.x + from.x) / 360f;
    }

    void Update()
    {
        float t = pulse(Time.time + Startingtime ,radiance);

        transform.eulerAngles = Vector3.Lerp(to, from, t);
    }

    //pulse between 0 and 1. for smooth movement of wrecking ball
    float pulse(float time,float rad)
    {
        const float frequency = 0.25f; // Frequency in Hz
        return rad * (1 + Mathf.Sin(2 * Mathf.PI * frequency * time));
    }

}















































/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBallScript : MonoBehaviour
{

    private Quaternion wreckingBallRotation;

    private bool isTurningRight;

    void Start()
    {
       
        StartCoroutine(SetWreckingBallPos());
        isTurningRight = false;
    }

    private void Update()
    {
        if (transform.localRotation.eulerAngles.x >= 315f)
        {
            isTurningRight = true;
        }
            
        else if (transform.localRotation.eulerAngles.x <= 225f)
            isTurningRight = false;
        Debug.Log("euler x " + transform.localRotation.eulerAngles.x + "isright" + isTurningRight);

    }

    IEnumerator SetWreckingBallPos()
    {
        while (true)
        {
            if (isTurningRight)
            {
                transform.Rotate(-2f, 0, 0);
            }
            else
            {
               transform.Rotate(2f, 0, 0);
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
}*/
