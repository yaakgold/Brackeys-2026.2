using Text;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PausePanelController : MonoBehaviour
{
    #region Singleton

    public static PausePanelController Instance { get; private set; }

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
    
    [SerializeField] private PanelRenderer panelRenderer;

    private VisualElement _root;
    
    void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _root = rootElement;

        _root.Q<Button>("btnCloseMenu").RegisterCallback<ClickEvent>(evt =>
        {
            _root.Q("pnlPause").style.display = DisplayStyle.None;
            GameManager.Instance.SetPaused(false);
        });
        
        _root.Q<Button>("btnOptions").RegisterCallback<ClickEvent>(evt => print("Need to make options panel"));
        
        _root.Q<Button>("btnLeaveGame").RegisterCallback<ClickEvent>(evt =>
        {
            MultiplayerService.Instance.Sessions["default-session"].LeaveAsync();
            SceneManager.LoadScene("Main Menu");
            Destroy(gameObject);
        });
        
        _root.Q<Button>("btnQuitGame").RegisterCallback<ClickEvent>(evt =>
        {
            Utilities.Quit();
        });
    }

    public void Open()
    {
        _root.Q("pnlPause").style.display = DisplayStyle.Flex;
    }
}
