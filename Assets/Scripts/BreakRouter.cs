using System;
using UnityEngine;

public class BreakRouter : Minigame
{
    private void Start()
    {
        ProjectManager.Instance.BreakRouterRpc();
        
        onCompleteMinigame.Invoke(0);
    }
}