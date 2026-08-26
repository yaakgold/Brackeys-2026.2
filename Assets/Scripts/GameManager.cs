using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    //TODO: Possibly make this a networked version of the singleton????
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
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private Transform target;
    
    private void Start()
    {
        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerPrefab, isPlayerObject: true);
        
        if (target == null)
            target = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;
        cam.Target.TrackingTarget = target;
    }
}
