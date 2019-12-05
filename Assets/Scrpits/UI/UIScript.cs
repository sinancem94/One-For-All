using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject levelPassedPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
        levelPassedPanel.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void LevelPassed()
    {
        levelPassedPanel.SetActive(true);
        Time.timeScale = 0;
    }
}
