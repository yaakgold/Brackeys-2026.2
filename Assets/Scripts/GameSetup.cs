using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : NetworkBehaviour
{
    public static GameSetup Instance { get; private set; }

    [SerializeField] private List<Color> playerColors;
    [SerializeField] private NetworkObject gameManagerPrefab;
    [SerializeField] private int timeDelay;
    
    private List<MpPlayerController> _playerControllers = new();
    
    public MpPlayerController GetPlayerController(ulong ownerId)
    {
        return _playerControllers.FirstOrDefault(p => p.OwnerClientId == ownerId);
    }

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
        
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
        
        var randomId = GetRandomId(NetworkManager.Singleton.ConnectedClientsIds);

        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var objs = NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(id);

            foreach (var obj in objs)
            {
                if (!obj.TryGetComponent(out MpPlayerController player)) continue;
                
                _playerControllers.Add(player);
                
                player.SetColorRpc(GetRandomColor());

                if (id == randomId)
                {
                    player.SetRoleRpc(EPlayerRole.BadGuy);
                }
                
                break;
            }
        }

        StartCoroutine(WaitForPlayerToReadInstructions());

    }

    private IEnumerator WaitForPlayerToReadInstructions()
    {
        yield return new WaitForSeconds(timeDelay);
        NetworkManager.Singleton.SceneManager.LoadScene("Main Game", LoadSceneMode.Single);
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadComplete;
    }

    private void SceneManagerOnOnLoadComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnOnLoadComplete;
        
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
            
        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(gameManagerPrefab);
        GameManager.Instance.SpawnPlayers();
        
    }
    
    private Color GetRandomColor()
    {
        var colorIndex = Random.Range(0, playerColors.Count);
        var color = playerColors[colorIndex];
        playerColors.Remove(color);
        return color;
    }

    private ulong GetRandomId(IReadOnlyList<ulong> ids)
    {
        var index = Random.Range(0, ids.Count);
        return ids[index];
    }
}
