using System;
using UnityEngine;

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

    public void SetCurrentInteractable(Interactable interactable)
    {
        currentInteractable = interactable;
    }
    
    public void UnSetCurrentInteractable()
    {
        currentInteractable = null;
    }

    public void Interact()
    {
        if (currentInteractable != null)
            currentInteractable.Interact();
    }
}
