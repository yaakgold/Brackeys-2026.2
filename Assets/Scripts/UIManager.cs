using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager Instance { get; private set; }

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
    
    [SerializeField] private EodPanelController eodPanelController;
    [SerializeField] private EndOfJamPanel eojPanelController;
    [SerializeField] private PanelRenderer inGameUIPanel;

    private PlayerMovement _player;
    private VisualElement _root;
    private Label _dayLabel;
    private Label _hourLabel;
    private ProgressBar _sanityProgressBar;

    private void OnEnable()
    {
        inGameUIPanel.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
    {
        _root = rootElement;
        
        _dayLabel = _root.Q<Label>("lblDay");
        _hourLabel = _root.Q<Label>("lblHour");
        _sanityProgressBar = _root.Q<ProgressBar>("pgbSanity");
    }

    public void OpenEndOfDayUI()
    {
        //TODO: Handle what to do if other players are in menus. I think I am going to force them out, like Among Us
        _player.GetComponent<PlayerInput>().enabled = false;
        eodPanelController.gameObject.SetActive(true);
    }

    public void CloseEndOfDayUI()
    {
        eodPanelController.gameObject.SetActive(false);
        _player.GetComponent<PlayerInput>().enabled = true;
    }

    public void OpenEndOfJamUI()
    {
        eojPanelController.gameObject.SetActive(true);
        _player.GetComponent<PlayerInput>().enabled = false;
    }

    public void SetDayText(int day)
    {
        
        _dayLabel.text = $"Day: {day}";
    }

    public void SetHourText(int hour)
    {
        _hourLabel.text = $"Hour: {hour}";
    }

    /// <summary>
    /// Set sanity
    /// </summary>
    /// <param name="sanity">New value / 100</param>
    public void SetSanity(int sanity)
    {
        _sanityProgressBar.value = sanity;
    }

    public void SetPlayer(PlayerMovement playerMovement)
    {
        _player = playerMovement;
    }
}
