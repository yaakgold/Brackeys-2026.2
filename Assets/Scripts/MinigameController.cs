using System;
using System.Collections;
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
    
    private void OnEnable()
    {
        Utilities.OnRestartApplication.AddListener(OnRestartApplication);
    }

    private void OnRestartApplication()
    {
        Utilities.OnRestartApplication.RemoveListener(OnRestartApplication);
        Destroy(gameObject);
    }
    
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
        value *= _task.isNegative ? -1 : 1;
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
                 var id = NetworkManager.Singleton.LocalClientId;
                 
                 if (PanelController.CurrentInteractable.HasPlayerName)
                 {
                     id = PanelController.CurrentInteractable.OwnerClientId;
                 }
                 ProjectManager.Instance.UpdateQualityRpc(value, id);
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

    public void CloseMinigame(int time = 1)
    {
        if (!_minigame.TryGetComponent(out Minigame mc)) return;
        mc.onCompleteMinigame.RemoveAllListeners();
        
        StartCoroutine(WaitASecondToClose(time));
    }

    private IEnumerator WaitASecondToClose(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(_minigame);
        renderMinigame.SetActive(false);
        panelController.ClosePanel();
    }
}