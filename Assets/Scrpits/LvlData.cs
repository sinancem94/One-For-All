using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlData 
{
    int Level;

    public LvlData(int level)
    {
        Level = level;
        SetData();
    }

    //DataMembers
    int memberCount;
    Vector3 gangPosition;

    public void SetData()
    {
        switch (Level)
        {
            case 1:
            default:

            memberCount = 25;
            gangPosition = Vector3.zero;

            break;

        }
    }

    public DataManager.LevelData GetLevelData()
    {
        DataManager.LevelData levelData = new DataManager.LevelData();

        levelData.motherGangPosition = gangPosition;
        levelData.memberCount = memberCount;

        return levelData;
    }

}