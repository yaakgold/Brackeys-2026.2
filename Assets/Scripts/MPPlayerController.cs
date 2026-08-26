using Unity.Netcode;
using UnityEngine;

public class MpPlayerController : NetworkBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private Color playerColor;

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
}
