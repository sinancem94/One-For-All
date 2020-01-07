using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    DataManager dataManager;
    void Awake()
    {
        dataManager = new DataManager();
    }

}
