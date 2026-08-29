using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using WebSocketSharp;

public class Interactable : NetworkBehaviour
{
    [SerializeField] private float interactionRange;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private PanelController uiPanel;
    [SerializeField] private string interactionName;
    [SerializeField] private List<GameTask> tasks;
    [SerializeField] private string playerName;
    
    public bool HasPlayerName => !playerName.IsNullOrEmpty();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerMovement player)) return;
        if (!player.IsOwner) return;
            
        canvas.gameObject.SetActive(true);
        interactionText.text = $"{interactionName}\nPress 'E' to interact";
        if (HasPlayerName)
        {
            interactionText.text = $"{playerName}'s " + interactionText.text;
        }
        InteractionManager.Instance.SetCurrentInteractable(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerMovement player)) return;
        if (!player.IsOwner) return;
        
        canvas.gameObject.SetActive(false);
        InteractionManager.Instance.UnSetCurrentInteractable();
    }

    public void Interact()
    {
        uiPanel.OpenPanel(this, tasks);
    }
    
    [Rpc(SendTo.Server)]
    public void SetNameRpc(string pName)
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
}
