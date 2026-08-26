using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

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

    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private List<Color> playerColors;
    
    private MpPlayerController _playerController;
    
    public MpPlayerController GetPlayerController() => _playerController;
    
    public override void OnNetworkSpawn()
    {
        if (Instance != this && Instance != null) return;
        
        base.OnNetworkSpawn();
        
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
        
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var objs = NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(id);

            foreach (var obj in objs)
            {
                if (!obj.TryGetComponent(out MpPlayerController player)) continue;
                
                _playerController = player;
                _playerController.SetColorRpc(GetRandomColor());
                break;
            }
            
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerPrefab, id, isPlayerObject: true);
        }
    }

    private Color GetRandomColor()
    {
        var colorIndex = Random.Range(0, playerColors.Count);
        var color = playerColors[colorIndex];
        playerColors.RemoveAt(colorIndex);
        return color;
    }
}
