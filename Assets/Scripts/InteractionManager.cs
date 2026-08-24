using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    #region Singleton

    public static InteractionManager Instance { get; private set; }

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
    
    [SerializeField] private Interactable currentInteractable;

    private PlayerMovement player;

    public void SetCurrentInteractable(Interactable interactable)
    {
        currentInteractable = interactable;
    }
    
    public void UnSetCurrentInteractable()
    {
        currentInteractable = null;
    }

    public void Interact(PlayerMovement p)
    {
        player = p;
        
        if (currentInteractable == null) return;
        
        player.GetComponent<PlayerInput>().enabled = false;
        currentInteractable.Interact();
    }

    public void FinishInteraction()
    {
        if (currentInteractable == null) return;
        player.GetComponent<PlayerInput>().enabled = true;
    }
}
