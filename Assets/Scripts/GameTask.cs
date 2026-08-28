using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Task", menuName = "Task", order = 0)]
public class GameTask : ScriptableObject
{
    public string title;
    public string description;
    public ETaskDifficulty difficulty;
    public ETaskType taskType;
    public int effectAmount;
    public int timeToComplete;
    public GameObject minigamePrefab;
    public bool checkIfRouterBrokenToday;

    private void OnValidate()
    {
        title = name;
    }
}