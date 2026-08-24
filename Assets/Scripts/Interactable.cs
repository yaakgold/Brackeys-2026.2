using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactable : NetworkBehaviour
{
    [SerializeField] private float interactionRange;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private PanelController uiPanel;
    [SerializeField] private string interactionName;
    [SerializeField] private List<GameTask> tasks;

    private void OnValidate()
    {
        GetComponent<CircleCollider2D>().radius = interactionRange;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerMovement player)) return;
        if (!player.IsOwner) return;
            
        canvas.gameObject.SetActive(true);
        interactionText.text = $"{interactionName}\nPress 'E' to interact";
        InteractionManager.Instance.SetCurrentInteractable(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerMovement player)) return;
        if (!player.IsOwner) return;
        
        canvas.gameObject.SetActive(false);
        InteractionManager.Instance.UnSetCurrentInteractable();
    }

    public virtual void Interact()
    {
        uiPanel.OpenPanel(this, tasks);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, interactionRange);
    }
}
