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
        DataScript.isGravityOpen = true;
        Time.timeScale = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
