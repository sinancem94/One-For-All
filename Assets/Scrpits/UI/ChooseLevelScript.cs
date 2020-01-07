using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseLevelScript : MonoBehaviour
{
    public void SetLevelAndRestart()
    {
        int level;

        Int32.TryParse(GetComponentInChildren<Text>().text, out level);

        Debug.Log(level);

        PlayerPrefs.SetInt("Level",level);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
