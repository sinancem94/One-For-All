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
    Transform member;
    int memberCount;
    Vector3 gangPosition;

    public void SetData()
    {
        GameObject Map;
        GameObject ground;

        switch (Level)
        {
            case 1:
                Map = LoadLevel(Level);

                ground = GameObject.FindGameObjectWithTag("Ground");

                member = Resources.Load<Transform>("Prefabs/GangMember");
                memberCount = 17;
                gangPosition = new Vector3(0f, ground.transform.position.y + 5f, 0f);

                break;

            case 2:
            case 3:
                Map = LoadLevel(Level);

                ground = GameObject.FindGameObjectWithTag("Ground");

                member = Resources.Load<Transform>("Prefabs/GangMember");
                memberCount = 7;
                gangPosition = new Vector3(0f, ground.transform.position.y + 5f, 0f);

                break;
            case 4:
                
                Map = LoadLevel(Level);

                ground = GameObject.FindGameObjectWithTag("Ground");

                member = Resources.Load<Transform>("Prefabs/GangMember");
                memberCount = 25;
                gangPosition = new Vector3(0f, ground.transform.position.y + 5f, 0f);

                break;
            case 5:
                Map = LoadLevel(Level);

                ground = GameObject.FindGameObjectWithTag("Ground");

                member = Resources.Load<Transform>("Prefabs/GangMember");
                memberCount = 8;
                gangPosition = new Vector3(0f, ground.transform.position.y + 5f, 0f);

                break;

            case 6:
                Map = LoadLevel(Level);

                ground = GameObject.FindGameObjectWithTag("Ground");

                member = Resources.Load<Transform>("Prefabs/GangMember");
                memberCount = 18;
                gangPosition = new Vector3(0f, ground.transform.position.y + 5f, 0f);

                break;
            
            default:

                Map = LoadLevel(1);

                ground = GameObject.FindGameObjectWithTag("Ground");

                member = Resources.Load<Transform>("Prefabs/GangMember");
                memberCount = 25;
                gangPosition = new Vector3(0f, ground.transform.position.y + 5f, 0f);

                break;

        }
    }

    GameObject LoadLevel(int level)
    {
        string mapStr = "Levels/Map" + level.ToString();

        Debug.Log("Loading " + mapStr);

        GameObject Map = (GameObject)Resources.Load(mapStr);
        return GameObject.Instantiate(Map);
    }

    public DataManager.LevelData GetLevelData()
    {
        DataManager.LevelData levelData = new DataManager.LevelData();

        levelData.motherGangPosition = gangPosition;
        levelData.memberCount = memberCount;
        levelData.memberToLoad = member;

        return levelData;
    }

}