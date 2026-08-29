using UnityEditor;
using UnityEngine;

public static class Utilities
{
    public static void Quit()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
        
        Application.Quit();
    }
}