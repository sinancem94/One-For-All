using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public static DataManager instance;

    string ME = "DataManager";
    public LevelData levelData;

    public DataManager()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError(ME + " already exist");
        }

        LvlData getLevelData;
        getLevelData = new LvlData(1);

        levelData = getLevelData.GetLevelData();
    }

    public MotherGang.GangState currentGangState;
    public MotherGang motherGang;

    public MotherGang.Gang GetGang()
    {
        return motherGang.GetGang();
    }

    public void SetMotherGang(MotherGang gang)
    {
        this.motherGang = gang;
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

