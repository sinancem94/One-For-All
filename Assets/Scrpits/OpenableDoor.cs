using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableDoor : MonoBehaviour, Interactable
{
    public enum OpenDirection
    {
        Left = -1,
        Right = 1
    }

    public OpenDirection openDirection;
    Vector2 direcion;

    private void Start()
    {
        direcion = new Vector2((int)openDirection, 0);
    }

    public void Interacted()
    {
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        float movementLength = transform.localScale.x;
        float movedLength = 0f;

        while(movedLength < movementLength)
        {
            Vector3 delta = (Vector3)direcion * 0.1f;

            transform.localPosition += delta;
            movedLength += delta.x;

            yield return new WaitForSecondsRealtime(0.01f);
        }

        yield return null;
    }
}
