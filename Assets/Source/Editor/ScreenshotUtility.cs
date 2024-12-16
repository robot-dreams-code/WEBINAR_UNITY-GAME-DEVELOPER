using System;
using UnityEditor;
using UnityEngine;

public static class ScreenshotUtility
{
    [MenuItem("Tools/Take Screenshot")]
    private static void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot($"Screenshots/{DateTime.Now:yyyyMMddHHmmssfff}.png");
    }
}