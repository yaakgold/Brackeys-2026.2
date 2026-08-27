using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class MpPlayerController : NetworkBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private Color playerColor;
    [SerializeField] private EPlayerRole playerRole;

    private int _sanity = 100;
    
    private void Start()
    {
        playerRole = EPlayerRole.Dev;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;
        
        SetNameRpc(AuthenticationService.Instance.PlayerName.Split("#")[0]);
    }

    public void UpdateSanity(int amt)
    {
        _sanity = Mathf.Clamp(_sanity + amt, 0, 100);
        UIManager.Instance.SetSanity(_sanity);
    }

    [Rpc(SendTo.Server)]
    private void SetNameRpc(string pName)
    {
        playerName = pName;
        SetNameLocalRpc(pName);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetNameLocalRpc(string pName)
    {
        name = pName;
        playerName = pName;
    }

    public Color GetName()
    {
        return playerColor;
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
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetRoleLocalRpc(EPlayerRole role)
    {
        playerRole = role;
    }

    public EPlayerRole GetRole()
    {
        return playerRole;
    }
}
