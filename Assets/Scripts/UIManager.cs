using UnityEngine;
using UnityEngine.InputSystem;
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
    private int _day;
    private int _hour;

    private void OnEnable()
    {
        inGameUIPanel.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDisable()
    {
        inGameUIPanel.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement)
    {
        _root = rootElement;
        
        _dayLabel = _root.Q<Label>("lblDay");
        _dayLabel.text = $"Day: {_day}";
        _hourLabel = _root.Q<Label>("lblHour");
        _hourLabel.text = $"Hour: {_hour}";
        _sanityProgressBar = _root.Q<ProgressBar>("pgbSanity");
    }
    
    public void OpenEndOfDayUI()
    {
        //TODO: Handle what to do if other players are in menus. I think I am going to force them out, like Among Us
        _player.GetComponent<PlayerInput>().enabled = false;
        eodPanelController.OpenPanel();
    }

    public void CloseEndOfDayUI()
    {
        eodPanelController.ClosePanel();
        _player.GetComponent<PlayerInput>().enabled = true;
    }

    public void OpenEndOfJamUI()
    {
        eojPanelController.OpenPanel();
        _player.GetComponent<PlayerInput>().enabled = false;
    }

    public void SetDayText(int day)
    {
        _day = day;
        if (_dayLabel != null)
        {
            _dayLabel.text = $"Day: {day}";
        }
    }

    public void SetHourText(int hour)
    {
        _hour = hour;
        if (_hourLabel != null)
        {
            _hourLabel.text = $"Hour: {hour}";
        }
    }

    /// <summary>
    /// Set sanity
    /// </summary>
    /// <param name="sanity">New value / 100</param>
    public void SetSanity(int sanity)
    {
        _sanityProgressBar.value = sanity;
    }
    
    public int GetSanity()
    {
        return Mathf.FloorToInt(_sanityProgressBar.value);
    }

    public void SetPlayer(PlayerMovement playerMovement)
    {
        _player = playerMovement;
    }
}
