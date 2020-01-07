using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableSpace : MonoBehaviour
{
    public Pushable MatchingPushable;
    public GameObject MatchingInteractable;

    Interactable thisInteractable;

    void Start()
    {
        Vector3 groundPosition;
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
      /*  if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, LayerMask.NameToLayer("Ground")))
        {
            groundPosition = hit.transform.position;

            groundPosition.y += hit.transform.localScale.y / 2f;
        }
        else
        {
            Debug.LogError("PushableSpace are not on any ground.");
            Transform ground = GameObject.FindGameObjectWithTag("Ground").transform;

            groundPosition = ground.position;

            groundPosition.y += ground.localScale.y / 2f;
        }*/

       // transform.position = new Vector3(transform.position.x, groundPosition.y + 1f, transform.position.z);

        SetSizeAccordingToPushable();

        thisInteractable = MatchingInteractable.GetComponent<Interactable>();
    }

    void SetSizeAccordingToPushable()
    {
        Vector3 scale = MatchingPushable.transform.localScale + (Vector3.one * 2f);

        scale.y = 1f;

        transform.localScale = scale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PushableObject") && MatchingPushable.gameObject.GetInstanceID() == other.gameObject.GetInstanceID())
        {
            thisInteractable.Interacted();
        }
    }

}
