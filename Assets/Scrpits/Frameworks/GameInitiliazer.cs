using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitiliazer : MonoBehaviour
{
    static GameInitiliazer Initialization;

    CameraSizeHandler camSizeHandler;
    void Start()
    {
        if (Initialization != null && Initialization != this)
            Destroy(this.gameObject);
        else
            Initialization = this;

        DontDestroyOnLoad(this);

#if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;
#elif UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
        QualitySettings.vSyncCount = 1;
        Time.timeScale = 1f;
#endif

        camSizeHandler = new CameraSizeHandler();
        camSizeHandler.SetCameraFieldOfView(Camera.main);
    }


    
}
