﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeHandler 
{
    //Fixed size is iPhoneX Size 2436x1125
    Vector2 fixedSize = new Vector2(1125, 2436);
    Vector2 deviceSize = new Vector2(Screen.width, Screen.height);

    //Fixed postion  
    Vector3 fixedPos = new Vector3(0f,1300,-1050);

    //fixed fof value (field of view)
    int fixedFof = 60;

    int currentFof = 0;

    string ME = "CameraSizeHandler";

    public void SetCameraFieldOfView(Camera mainCam)
    {
        currentFof = CalculateFofAccordingtoScreen();

        if (mainCam)
            mainCam.fieldOfView = currentFof;
        else
            Debug.LogError( ME + ": No gameobject with tag MainCamera.");
    }

    int CalculateFofAccordingtoScreen()
    {
        //Field of view maximum value is 75
        int maxFof = fixedFof + 15;
        //Field of view minimum value is 45
        int minFof = fixedFof - 15;

        //Iphone x height / width . Bununla simdiki ratio yu karsilastir. 
        float fixedScreenRatio = fixedSize.y / fixedSize.x;
        float screenRatio = deviceSize.y / deviceSize.x;

        //Ratio ile fof dogru orantili
        //fixedScreenRatio * currentFof = currScreenRatio * fixedFof
        int fof = (int)((screenRatio * fixedFof) / fixedScreenRatio);

        //if field of view is not on presetted bounderies 
        if (fof > maxFof)
            fof = maxFof;
        if (fof < minFof)
            fof = minFof;


        return fof;
    }
}
