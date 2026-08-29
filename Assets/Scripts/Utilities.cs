using System.Threading.Tasks;
using Unity.Services.Multiplayer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class Utilities
{
    public static UnityEvent OnRestartApplication;
    
    public static void Quit()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
        
        Application.Quit();
    }

    public static void LeaveSession()
    {
        _ = LeaveSessionAsync();
    }

    private static async Task LeaveSessionAsync()
    {
        var leaveTask = MultiplayerService.Instance?.Sessions["default-session"]?.LeaveAsync();
        if (leaveTask != null)
            await leaveTask;   
    }
}