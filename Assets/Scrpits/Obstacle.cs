using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int manCount;

    private void Start()
    {
        if(manCount == 0)
        {
            Debug.LogError("Man count of obstacle " + this.name + " is not setted.");
        }
    }
}
