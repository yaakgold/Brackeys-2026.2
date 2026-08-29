using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelController : MonoBehaviour
{
    [SerializeField] private PanelRenderer panelRenderer;

    private VisualElement _root;
    private VisualElement _panel;
    private MultiColumnListView _taskListView;
    private Button _closeButton;
    
    private static Interactable _currentInteractable;
    
    public static Interactable CurrentInteractable => _currentInteractable;
    
    void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }
    
    void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _root = rootElement;

        _panel = _root.Q<VisualElement>("pnlDisplay");

        _closeButton = _root.Q<Button>("btnClose");
        _closeButton.RegisterCallback<ClickEvent>(e => ClosePanel());

        _taskListView = _root.Q<MultiColumnListView>("mclvTasks");
        _taskListView.columns[0].makeCell = () => new Label();
        _taskListView.columns[1].makeCell = () => new Button();
        
        //Task Name
        _taskListView.columns[0].bindCell = (element, i) =>
        {
            ((Label)element).text = ((GameTask)_taskListView.itemsSource[i]).title;
            ((Label)element).AddToClassList("blocks-label");
        };
        
        //Do Task button
        _taskListView.columns[1].bindCell = (element, i) =>
        {
            ((Button)element).text = "Do Task";
            ((Button)element).AddToClassList("blocks-button");
            ((Button)element).RegisterCallbackOnce<ClickEvent>(evt => OnDoTaskClicked(((GameTask)_taskListView.itemsSource[i])));
        };
    }

    public void OpenPanel(Interactable interactable, List<GameTask> tasks)
    {
        _panel.style.display = DisplayStyle.Flex;

        _taskListView ??= _root.Q<MultiColumnListView>("mclvTasks");
        
        _currentInteractable = interactable;

        _taskListView.itemsSource = tasks;
        _taskListView.Rebuild();
    }

    private void OnDoTaskClicked(GameTask task)
    {
        if (task.minigamePrefab == null) return;
        
        MinigameController.Instance.StartMinigame(task.minigamePrefab, task);
    }

    public void ClosePanel()
    {
        InteractionManager.Instance.FinishInteraction();
        
        _panel.style.display = DisplayStyle.None;
        _taskListView.itemsSource = null;
        _taskListView.RefreshItems();
    }

    public void HidePanel()
    {
        _panel.style.display = DisplayStyle.None;
    }
}
