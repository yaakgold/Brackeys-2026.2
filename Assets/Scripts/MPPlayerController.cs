using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;

public class MpPlayerController : NetworkBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private Color playerColor;
    [SerializeField] private EPlayerRole playerRole;

    private int _sanity = 100;
    
    public UnityEvent<EPlayerRole> onPlayerRoleChanged;
    
    private void Start()
    {
        playerRole = EPlayerRole.Developer;
    }
    
    private void OnEnable()
    {
        Utilities.OnRestartApplication.AddListener(OnRestartApplication);
    }

    private void OnRestartApplication()
    {
        Utilities.OnRestartApplication.RemoveListener(OnRestartApplication);
        Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Utilities.OnMpPlayerSpawned.Invoke(OwnerClientId);
        if (!IsOwner) return;
        
        SetNameRpc(AuthenticationService.Instance.PlayerName);
        
        AuthenticationService.Instance.PlayerNameChanged += SetNameRpc;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
        AuthenticationService.Instance.PlayerNameChanged -= SetNameRpc;
    }

    public void UpdateSanity(int amt)
    {
        _sanity = Mathf.Clamp(_sanity + amt, 0, 100);
        UIManager.Instance.SetSanity(_sanity);
    }

    [Rpc(SendTo.Server)]
    private void SetNameRpc(string pName)
    {
        playerName = pName.Split("#")[0];
        SetNameLocalRpc(playerName);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetNameLocalRpc(string pName)
    {
        name = pName;
        playerName = pName;
    }

    public string GetName()
    {
        return playerName;
    }
    
    [Rpc(SendTo.Server)]
    public void SetColorRpc(Color color)
    {
        playerColor = color;
        SetColorLocalRpc(color);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetColorLocalRpc(Color color)
    {
        playerColor = color;
    }

    public Color GetColor()
    {
        return playerColor;
    }
    
    [Rpc(SendTo.Server)]
    public void SetRoleRpc(EPlayerRole role)
    {
        playerRole = role;
        SetRoleLocalRpc(role);
        onPlayerRoleChanged?.Invoke(playerRole);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetRoleLocalRpc(EPlayerRole role)
    {
        playerRole = role;
        
        onPlayerRoleChanged?.Invoke(playerRole);
    }

    public EPlayerRole GetRole()
    {
        return playerRole;
    }
}
