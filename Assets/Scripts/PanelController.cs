using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject taskPanel;

    private Interactable _currentInteractable;
    private List<GameObject> _taskPanels;
    
    private void Start()
    {
        closeButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel(Interactable interactable, List<GameTask> tasks)
    {
        gameObject.SetActive(true);
        _taskPanels ??= new List<GameObject>();
        
        _currentInteractable = interactable;

        foreach (var task in tasks)
        {
            var obj = Instantiate(taskPanel, transform);

            if (obj.TryGetComponent(out TaskUI ui))
            {
                ui.SetTask(task);
            }
            
            _taskPanels.Add(obj);
        }
    }

    private void ClosePanel()
    {
        foreach (var obj in _taskPanels)
        {
            Destroy(obj);
        }
        
        _taskPanels.Clear();
        
        InteractionManager.Instance.FinishInteraction();
        gameObject.SetActive(false);
    }
}
