using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactable : NetworkBehaviour
{
    [SerializeField] private float interactionRange;
    [SerializeField] private Canvas canvas;

    private void OnValidate()
    {
        GetComponent<CircleCollider2D>().radius = interactionRange;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        canvas.gameObject.SetActive(true);
        InteractionManager.Instance.SetCurrentInteractable(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canvas.gameObject.SetActive(false);
        InteractionManager.Instance.UnSetCurrentInteractable();
    }

    public virtual void Interact()
    {
        print($"{name} is being interacted with");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, interactionRange);
    }
}
