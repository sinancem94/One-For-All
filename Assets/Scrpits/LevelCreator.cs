using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    DataManager dataManager;
    void Awake()
    {
        dataManager = new DataManager();
        DataScript.memberCollisionLock = false;
        DataScript.inputLock = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
