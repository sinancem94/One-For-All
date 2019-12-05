using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public static DataManager instance;

    public string ME = "DataManager";
    public LevelData levelData;

    public DataManager()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
           // Debug.LogError(ME + " already exist");
        }

        LvlData getLevelData;
        getLevelData = new LvlData(1);

        levelData = getLevelData.GetLevelData();

    }

    //Current level data
    public struct LevelData
    {
       public Vector3 motherGangPosition; 
       public int memberCount;
    }
   
    //Player specific data will be managed in here. 
    //Current level. If any purchasable is purchased etc.
    public struct PlayerData
    {
        
    }
}

