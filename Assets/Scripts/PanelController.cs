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
    private bool _isRouterBroken;
    
    private const string DoTask = "Do Task";
    private const string FixRouter = "Fix Router";
    
    public static Interactable CurrentInteractable => _currentInteractable;
    
    void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
        ProjectManager.Instance.onRouterBroken.AddListener(OnRouterBroken);
        ProjectManager.Instance.onRouterFixed.AddListener(OnRouterFixed);
    }

    void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        ProjectManager.Instance.onRouterBroken.RemoveListener(OnRouterBroken);
        ProjectManager.Instance.onRouterFixed.RemoveListener(OnRouterFixed);
    }
    
    private void OnRouterBroken()
    {
        _isRouterBroken = true;
    }
    
    private void OnRouterFixed()
    {
        _isRouterBroken = false;
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
            var gameTask = (GameTask)_taskListView.itemsSource[i];
            if (_isRouterBroken && gameTask.checkIfRouterIsCurrentlyBroken)
            {
                ((Button)element).text = FixRouter;
            }
            else
            {
                ((Button)element).text = "Do Task";
            }
            ((Button)element).AddToClassList("blocks-button");
            ((Button)element).RegisterCallbackOnce<ClickEvent>(evt => OnDoTaskClicked(gameTask));

            if (!gameTask.checkIfRouterIsCurrentlyBroken) return;

            var dataBinding = new DataBinding
            {
                bindingMode = BindingMode.ToTarget,
                dataSource = !_isRouterBroken
            };
            
            ((Button)element).SetBinding("enabledSelf", dataBinding);
            ((Button)element).dataSource = !_isRouterBroken;
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
