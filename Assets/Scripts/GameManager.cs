using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private float tickSpeed;

    public int sanityDecreaseAmount;
    public bool IsPaused { get; private set; } = false;
    public UnityEvent onTick;

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
            return;
        }
        
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
        
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerPrefab, id, isPlayerObject: true);
        }

        StartCoroutine(Tick());
        ProjectManager.Instance.UpdateDayRpc();
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitUntil(() => !IsPaused);
            yield return new WaitForSeconds(tickSpeed);
            onTick.Invoke();
        }
    }
    
    public void SetPaused(bool paused)
    {
        IsPaused = paused;
    }
}
