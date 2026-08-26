using System;
using Blocks.Sessions.Common;
using Unity.Netcode;
using Unity.Properties;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WebSocketSharp;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkObject projectManagerPrefab;
    [SerializeField] private NetworkObject gameManagerPrefab;
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private SessionSettings sessionSettings;
    
    private VisualElement _root;
    private SessionObserver _sessionObserver;
    private ISession _session;

    private void Start()
    {
        _sessionObserver = new SessionObserver(sessionSettings.sessionType);
        
        _sessionObserver.AddingSessionStarted += SessionObserverOnAddingSessionStarted; 
        _sessionObserver.AddingSessionFailed += SessionObserverOnAddingSessionFailed;
        _sessionObserver.SessionAdded += SessionObserverOnSessionAdded;
    }
    
    void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }
    
    private void SessionObserverOnSessionAdded(ISession obj)
    {
        _session = obj;

        var playerName = _root.Q<TextField>("playerName")?.value;
        if (!playerName.IsNullOrEmpty())
            AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        
        _session.RemovedFromSession += SessionOnRemovedFromSession;
        _session.PlayerJoined += SessionOnPlayerJoined;
        _session.PlayerLeaving += SessionOnPlayerLeaving;
        
        if (!_session.IsHost) return;
        _root.Q("btnStart").style.display = DisplayStyle.Flex;
            
        //TODO: Remove this after testing
        _root.Q("btnStart").enabledSelf = true;
    }

    private void SessionOnPlayerLeaving(string obj)
    {
        if (_session.PlayerCount <= 2)
        {
            _root.Q("btnStart").enabledSelf = false;
        }
    }

    private void SessionOnRemovedFromSession()
    {
        _root.Q("join").style.display = DisplayStyle.Flex;
        _root.Q("currentSession").style.display = DisplayStyle.None;
    }

    private void SessionObserverOnAddingSessionStarted(AddingSessionOptions obj)
    {
        _root.Q("join").style.display = DisplayStyle.None;
        _root.Q("currentSession").style.display = DisplayStyle.Flex;
    }

    private void SessionObserverOnAddingSessionFailed(AddingSessionOptions arg1, SessionException arg2)
    {
        _root.Q("join").style.display = DisplayStyle.Flex;
        _root.Q("currentSession").style.display = DisplayStyle.None;
    }

    private void SessionOnPlayerJoined(string obj)
    {
        if (_session.PlayerCount >= 2)
        {
            _root.Q("btnStart").enabledSelf = true;
        }
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _root = rootElement;
        
        _root.Q<Button>("btnJoin").RegisterCallback<ClickEvent>(OnJoin);
        _root.Q<Button>("btnExit").RegisterCallback<ClickEvent>(OnExit);
        
        _root.Q<Button>("btnStart").RegisterCallback<ClickEvent>(StartGame);
        
        _root.Q("quickJoin").RegisterCallback<ClickEvent>(OnDisplayChange);
    }

    private void OnDisplayChange(ClickEvent evt)
    {
        var obj = _root.Q("quickJoin");
        
        _root.Q("currentSession").style.display = obj.style.display== DisplayStyle.Flex ?  DisplayStyle.None : DisplayStyle.Flex;
    }

    private void OnExit(ClickEvent evt)
    {
        Application.Quit();
    }

    private void OnJoin(ClickEvent evt)
    {
        _root.Q("mainMenu").style.display = DisplayStyle.None;
        _root.Q("join").style.display = DisplayStyle.Flex;
    }
    
    void StartGame(ClickEvent evt)
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Main Game", LoadSceneMode.Single);
        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(projectManagerPrefab);
        NetworkManager.Singleton.SceneManager.OnLoadComplete += (id, sceneName, mode) =>
        {
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
            
            print("Running");
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(gameManagerPrefab);
        };
    }
}