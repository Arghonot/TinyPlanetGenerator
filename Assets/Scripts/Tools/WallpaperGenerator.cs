using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class generate a screenshot.
/// </summary>
public class WallpaperGenerator : MonoBehaviour
{
    public string Name;
    public bool TakeScreenshot;

    string _Path = "/Wallpapers/";

    void Update()
    {
        if (TakeScreenshot)
        {
            TakeScreenshot = false;

            HandleScreenshot();
        }
    }

    void HandleScreenshot()
    {
        ScreenCapture.CaptureScreenshot(Application.dataPath + _Path + Name + ".png", 4);
    }
}
