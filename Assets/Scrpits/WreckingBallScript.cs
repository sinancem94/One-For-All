using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBallScript : MonoBehaviour
{

    //float speed;
    Vector3 orgRotation;

    Vector3 to;
    Vector3 from;

    float radiance; 

    void Start()
    {
        orgRotation = transform.rotation.eulerAngles;

        to = new Vector3(-20f, 90f, 0f);
        from = new Vector3(-160f, 90f, 0f);

        radiance = Mathf.Abs(to.x + from.x) / 360f;
    }

    void Update()
    {
        //float diff = Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.x, orgRotation.x));
        //speed = normalizeDistance(diff);

        float t = pulse(Time.time,radiance);
        //float t = Mathf.Abs( Mathf.Sin( ( Time.time * Mathf.PI) / 180  ) ) * speed;

        //float t = Mathf.PingPong(timer * speed, 1.0f);
        transform.eulerAngles = Vector3.Lerp(to, from, t);

        //transform.rotation = Quaternion.Lerp(rotation1, rotation2, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
    }

    //pulse between 0 and 1. for smooth movement of wrecking ball
    float pulse(float time,float rad)
    {
        const float frequency = 0.25f; // Frequency in Hz
        return rad * (1 + Mathf.Sin(2 * Mathf.PI * frequency * time));
    }

    
    //normalize distance of vectors between zero and 1
    float normalizeDistance(float distance)
    {
        float Max = Mathf.Abs(Mathf.DeltaAngle(orgRotation.x, from.x));

        float Min = 0f;

        float normDist = (Max - distance) / (Max - Min);

        return normDist; //((normalizedMax - normalizedMin) / (Max - Min) * (distance - Max)) + normalizedMax;
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
