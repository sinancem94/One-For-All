using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject levelPassedPanel;
    public GameObject chooseLevelPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
        levelPassedPanel.SetActive(false);
        chooseLevelPanel.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void LevelPassed()
    {
        levelPassedPanel.SetActive(true);
    }

    public void OpenLevelPanel()
    {
        levelPassedPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        chooseLevelPanel.SetActive(true);
    }
}
