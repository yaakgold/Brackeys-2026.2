using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ProjectManager : NetworkBehaviour
{
    #region Singleton

    public static ProjectManager Instance { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
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

    [SerializeField] private int qualityLevel;
    [SerializeField] private int dayNumber;
    [SerializeField] private int hour;
    [SerializeField] private int completionAmount;
    [SerializeField] private int numWordsToResetRouter;
    [SerializeField] private int jamLength;

    private int _numWordsLeft;
    
    public UnityEvent onRouterFixed;
    public UnityEvent onRouterBroken;
    public UnityEvent<int> onNumWordsChanged;
    
    public int GetDay() => dayNumber;
    public int GetNumWords() => _numWordsLeft;
    
    public bool RouterBrokenToday { get; private set; }
    
    public readonly Dictionary<ulong, int> DictTaskLog = new();
    
    //Handle quality
    [Rpc(SendTo.Server)]
    public void UpdateQualityRpc(int qualityAdd, ulong ownerClientId)
    {
        qualityLevel += qualityAdd;
        SetQualityRpc(qualityLevel, ownerClientId);
        
        if (DictTaskLog.TryGetValue(ownerClientId, out var taskLog))
        {
            DictTaskLog[ownerClientId] += qualityAdd;
        }
        else
        {
            DictTaskLog[ownerClientId] = qualityAdd;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetQualityRpc(int q, ulong ownerClientId)
    {
        if (DictTaskLog.TryGetValue(ownerClientId, out var taskLog))
        {
            DictTaskLog[ownerClientId] += q - qualityLevel;
        }
        else
        {
            DictTaskLog[ownerClientId] = q - qualityLevel;
        }
        
        qualityLevel = q;
    }
    
    //Handle Day
    [Rpc(SendTo.Server)]
    public void UpdateDayRpc()
    {
        dayNumber++;
        SetDayRpc(dayNumber);
        RouterBrokenToday = false;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetDayRpc(int currentDay)
    {
        dayNumber = currentDay;
        UIManager.Instance.SetDayText(dayNumber);
        RouterBrokenToday = false;
    }
    
    //Handle quality
    [Rpc(SendTo.Server)]
    public void UpdateTimeRpc(int timeAdd)
    {
        hour += timeAdd;
        
        if (hour >= 24)
        {
            EndOfDayRpc();
            hour = 0;
            UpdateDayRpc();
        }
        
        SetTimeRpc(hour);
    }
    
    //Handle num words left
    [Rpc(SendTo.Server)]
    public void UpdateWordsRpc()
    {
        _numWordsLeft--;
        if (_numWordsLeft <= 0)
        {
            onRouterFixed.Invoke();
        }
        onNumWordsChanged.Invoke(_numWordsLeft);
        SetWordsRpc(_numWordsLeft);
    }
    
    [Rpc(SendTo.Server)]
    public void UpdateWordsRpc(int numWords)
    {
        _numWordsLeft = numWords;
        if (_numWordsLeft <= 0)
        {
            onRouterFixed.Invoke();
        }
        onNumWordsChanged.Invoke(_numWordsLeft);
        SetWordsRpc(_numWordsLeft);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetWordsRpc(int w)
    {
        _numWordsLeft = w;
        if (_numWordsLeft <= 0)
        {
            onRouterFixed.Invoke();
        }
        onNumWordsChanged.Invoke(_numWordsLeft);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetTimeRpc(int time)
    {
        hour = time;
        UIManager.Instance.SetHourText(hour);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void EndOfDayRpc()
    {
        if (dayNumber == jamLength)
        {
            UIManager.Instance.OpenEndOfJamUI();
        }
        else
        {
            UIManager.Instance.OpenEndOfDayUI();
        }
    }

    [Rpc(SendTo.Server)]
    public void StartNextDayRpc()
    {
        UIManager.Instance.CloseEndOfDayUI();
        
        StartNextDayLocalRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartNextDayLocalRpc()
    {
        UIManager.Instance.CloseEndOfDayUI();
    }

    [Rpc(SendTo.Server)]
    public void BreakRouterRpc()
    {
        UpdateWordsRpc(numWordsToResetRouter);
        onRouterBroken.Invoke();
        BreakRouterClientRpc();
        RouterBrokenToday = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BreakRouterClientRpc()
    {
        onRouterBroken.Invoke();
        RouterBrokenToday = true;
    }
}
