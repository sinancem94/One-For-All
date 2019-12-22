using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public static DataManager instance;

    string ME = "DataManager";
    public LevelData levelData;

    public UIScript UI;

    //Dynamic LevelData
    public GangState currentGangState;
    public MotherGang motherGang;

    private GameState gState;
    public GameState gameState
    {
        get { return gState; }   // get method
    }

    public enum GameState
    {
        Play,
        End
    }

    public enum GangState
    {
        Idle = 0,
        Walking,
        Climbing,
        Bridge,
        LevelPassed,
        GameOver
    };

    public DataManager()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError(ME + " already exist");

            instance = this;
        }

        LvlData getLevelData;
        getLevelData = new LvlData(1);

        levelData = getLevelData.GetLevelData();

        SetState(GameState.Play);

        UI = GameObject.FindObjectOfType(typeof(UIScript)) as UIScript;
        motherGang = GameObject.FindObjectOfType(typeof(MotherGang)) as MotherGang;
    }

    void SetState(GameState newState)
    {
        gState = newState;
    }

    public void LevelPassed()
    {
        SetState(GameState.End);

        Time.timeScale = 0;
        UI.LevelPassed();

        currentGangState = GangState.LevelPassed;

        resetDataManager();
    }

    public void GameOver()
    {
        SetState(GameState.End);

        Time.timeScale = 0;
        UI.GameOver();

        currentGangState = GangState.GameOver;

        resetDataManager();
    }

    void resetDataManager()
    {
        
    }

    public MotherGang.Gang GetGang()
    {
        return motherGang.GetGang();
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

