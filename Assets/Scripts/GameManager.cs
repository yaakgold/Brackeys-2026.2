using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

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
        
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost) return;
        
        StartCoroutine(Tick());
        ProjectManager.Instance.UpdateDayRpc();
    }

    protected override void OnNetworkPostSpawn()
    {
        base.OnNetworkPostSpawn();

        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost) return;
        
        var tempComputers = GameObject.FindGameObjectsWithTag("Computer").ToList();
        var computerOwnerSet = new List<GameObject>();
        
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var randomComputerIndex = Random.Range(0, tempComputers.Count);
            if (tempComputers[randomComputerIndex].TryGetComponent(out Interactable interactable))
            {
                interactable.NetworkObject.ChangeOwnership(id);
                
                interactable.SetNameRpc(NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id).name);
            }
            tempComputers[randomComputerIndex].gameObject.SetActive(true);
            computerOwnerSet.Add(tempComputers[randomComputerIndex]);
            
            tempComputers.RemoveAt(randomComputerIndex);
        }
        
        foreach (var computer in tempComputers.Where(c => !computerOwnerSet.Contains(c)))
        {
            if (computer.TryGetComponent(out Interactable interactable))
            {
                interactable.NetworkObject.Despawn();
            }
        }
    }

    public void SpawnPlayers()
    {
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
        
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerPrefab, id, isPlayerObject: true);
        }
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitUntil(() => !IsPaused);
            yield return new WaitForSeconds(tickSpeed);
            onTick.Invoke();
            TickLocalRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TickLocalRpc()
    {
        onTick.Invoke();
    }
    
    public void SetPaused(bool paused)
    {
        IsPaused = paused;
    }
}
