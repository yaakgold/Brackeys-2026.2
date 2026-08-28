using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    #region Singleton

    public static MinigameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion
    
    [SerializeField] private GameObject renderMinigame;
    [SerializeField] private Vector2 prefabSpawnPos;
    [SerializeField] private PanelController panelController;

    private GameObject _minigame;
    private GameTask _task;
    
    public void StartMinigame(GameObject minigame, GameTask task)
    {
        _task = task;
        _minigame = Instantiate(minigame, prefabSpawnPos, Quaternion.identity);

        panelController.HidePanel();

        if (!_minigame.TryGetComponent(out Minigame mc)) return;
        
        mc.onCompleteMinigame.AddListener(EndMinigame);

        if (!mc.requiresRenderer) return;
        
        renderMinigame.GetComponent<MeshRenderer>().material = mc.minigameMaterial;
        renderMinigame.SetActive(true);
    }

    private void EndMinigame(int value)
    {
         switch (_task.taskType)
         {
             case ETaskType.Sanity:
                 var objs = NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(NetworkManager.Singleton.LocalClientId);
                 foreach (var networkObject in objs)
                 {
                     if (!networkObject.TryGetComponent(out MpPlayerController player)) continue;
                     player.UpdateSanity(value);
                 }
                 break;
             case ETaskType.Quality:
                 ProjectManager.Instance.UpdateQualityRpc(value, NetworkManager.Singleton.LocalClientId);
                 break;
             case ETaskType.Sabotage:
                 break;
             case ETaskType.Other:
                 break;
             default:
                 throw new ArgumentOutOfRangeException();
         }
        ProjectManager.Instance.UpdateTimeRpc(_task.timeToComplete);
        
        CloseMinigame();
    }

    public void CloseMinigame()
    {
        Destroy(_minigame);
        renderMinigame.SetActive(false);
        panelController.ClosePanel();
    }
}