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

            GameObject Map = LoadLevel(1);

            GameObject ground = GameObject.FindGameObjectWithTag("Ground");

            memberCount = 25;
            gangPosition = new Vector3(0f, ground.transform.position.y + 5f, 0f);

            
            break;

        }
    }

    GameObject LoadLevel(int level)
    {
        string mapStr = "Prefabs/Map" + level.ToString();

        Debug.Log("Creating " + mapStr);

        GameObject Map = (GameObject)Resources.Load(mapStr);
        return GameObject.Instantiate(Map);
    }

    public DataManager.LevelData GetLevelData()
    {
        DataManager.LevelData levelData = new DataManager.LevelData();

        levelData.motherGangPosition = gangPosition;
        levelData.memberCount = memberCount;

        return levelData;
    }

}