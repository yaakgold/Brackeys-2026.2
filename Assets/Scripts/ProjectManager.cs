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

    public int GetDay() => dayNumber;
    
    //Handle quality
    [Rpc(SendTo.Server)]
    public void UpdateQualityRpc(int qualityAdd)
    {
        qualityLevel += qualityAdd;
        SetQualityRpc(qualityLevel);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetQualityRpc(int q)
    {
        qualityLevel = q;
    }
    
    //Handle Day
    [Rpc(SendTo.Server)]
    public void UpdateDayRpc()
    {
        dayNumber++;
        SetDayRpc(dayNumber);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetDayRpc(int currentDay)
    {
        dayNumber = currentDay;
        UIManager.Instance.SetDayText(dayNumber);
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

    [Rpc(SendTo.ClientsAndHost)]
    private void SetTimeRpc(int time)
    {
        hour = time;
        UIManager.Instance.SetHourText(hour);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void EndOfDayRpc()
    {
        UIManager.Instance.OpenEndOfDayUI();
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
}
