using System;
using System.Collections.Generic;
using Blocks.Sessions.Common;
using Text;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WebSocketSharp;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkObject projectManagerPrefab;
    [SerializeField] private NetworkObject gameSetupPrefab;
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private SessionSettings sessionSettings;
    [SerializeField] private Texture[] instructions;
    [SerializeField] private int minPlayers;
    
    private VisualElement _root;
    private SessionObserver _sessionObserver;
    private ISession _session;
    private int _currentIndex;
    private int _currentConnectedPlayers;

    private void Awake()
    {
        Utilities.OnRestartApplication = new UnityEvent();
        Utilities.OnMpPlayerSpawned = new UnityEvent<ulong>();
    }

    private void Start()
    {
        _sessionObserver = new SessionObserver(sessionSettings.sessionType);
        
        _sessionObserver.AddingSessionStarted += SessionObserverOnAddingSessionStarted; 
        _sessionObserver.AddingSessionFailed += SessionObserverOnAddingSessionFailed;
        _sessionObserver.SessionAdded += SessionObserverOnSessionAdded;
    }

    private void OnDestroy()
    {
        _sessionObserver.AddingSessionStarted -= SessionObserverOnAddingSessionStarted; 
        _sessionObserver.AddingSessionFailed -= SessionObserverOnAddingSessionFailed;
        _sessionObserver.SessionAdded -= SessionObserverOnSessionAdded;    
    }

    void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
        Utilities.OnMpPlayerSpawned.AddListener(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(ulong clientId)
    {
        _currentConnectedPlayers++;
        CheckIfCanStart();
    }

    void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }
    
    private void SessionObserverOnSessionAdded(ISession obj)
    {
        _session = obj;

        var playerName = _root.Q<TextField>("playerName")?.value;
        if (playerName != null && !playerName.IsNullOrEmpty())
        {
            playerName = playerName.Replace(' ', '_');
            AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        }

        ChatManager.Instance.EnableChat();
        
        _session.RemovedFromSession += SessionOnRemovedFromSession;
        _session.PlayerJoined += SessionOnPlayerJoined;
        _session.PlayerLeaving += SessionOnPlayerLeaving;
        
        if (!_session.IsHost) return;
        _root.Q("btnStart").style.display = DisplayStyle.Flex;
        
        CheckIfCanStart();
    }

    private void SessionOnPlayerLeaving(string obj)
    {
        if (SceneManager.GetActiveScene().name != "Main Menu") return;
        _currentConnectedPlayers--;
        
        CheckIfCanStart();
    }

    private void SessionOnRemovedFromSession()
    {
        _currentConnectedPlayers = 0;
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            SceneManager.LoadScene("Main Menu");
            Utilities.OnRestartApplication.Invoke();
        }
        else
        {
            _root.Q("join").style.display = DisplayStyle.Flex;
            _root.Q("currentSession").style.display = DisplayStyle.None;
            _root.Q<Button>("btnBack").style.display = DisplayStyle.Flex;
            _root.Q("btnStart").style.display = DisplayStyle.None;
            ChatManager.Instance.DisableChat();   
        }
    }

    private void SessionObserverOnAddingSessionStarted(AddingSessionOptions obj)
    {
        _root.Q("join").style.display = DisplayStyle.None;
        _root.Q("currentSession").style.display = DisplayStyle.Flex;
        _root.Q<Button>("btnBack").style.display = DisplayStyle.None;
    }

    private void SessionObserverOnAddingSessionFailed(AddingSessionOptions arg1, SessionException arg2)
    {
        _root.Q("join").style.display = DisplayStyle.Flex;
        _root.Q("currentSession").style.display = DisplayStyle.None;
        _root.Q<Button>("btnBack").style.display = DisplayStyle.Flex;
    }

    private void SessionOnPlayerJoined(string obj)
    {
        if (SceneManager.GetActiveScene().name != "Main Menu") return;

        CheckIfCanStart();
    }

    private void CheckIfCanStart()
    {
        _root.Q("btnStart").enabledSelf = _currentConnectedPlayers >= minPlayers;
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _root = rootElement;
        
        _root.Q<Button>("btnJoin").RegisterCallback<ClickEvent>(OnJoin);
        _root.Q<Button>("btnExit").RegisterCallback<ClickEvent>(OnExit);
        _root.Q<Button>("btnBack").RegisterCallback<ClickEvent>(OnBack);
        
        _root.Q<Button>("btnStart").RegisterCallback<ClickEvent>(StartGame);
        
        _root.Q("QuickJoinButton").RegisterCallback<ClickEvent>(OnDisplayChange);

        var playerName = AuthenticationService.Instance.PlayerName.Split('#')[0];
        _root.Q<TextField>("playerName").value = playerName == "" ?  "" : playerName;
        
        _root.Q<Button>("btnNext").RegisterCallback<ClickEvent>(OnInstructionNext);
        _root.Q<Button>("btnInstructions").RegisterCallback<ClickEvent>(_ =>
        {
            _currentIndex = 0;
            _root.Q<Image>("imgInstruction").image = instructions[_currentIndex];
            _root.Q("pnlInstructions").style.display = DisplayStyle.Flex;
        });
    }

    private void OnBack(ClickEvent evt)
    {
        _root.Q<Button>("btnBack").style.display = DisplayStyle.None;
        _root.Q("mainMenu").style.display = DisplayStyle.Flex;
        _root.Q("join").style.display = DisplayStyle.None;
    }

    private void OnInstructionNext(ClickEvent evt)
    {
        _currentIndex++;
        if (_currentIndex >= instructions.Length)
        {
            _root.Q("pnlInstructions").style.display = DisplayStyle.None;
            return;
        }
        
        _root.Q<Image>("imgInstruction").image = instructions[_currentIndex];
    }

    private void OnDisplayChange(ClickEvent evt)
    {
        var obj = _root.Q("join");
        
        _root.Q("currentSession").style.display = obj.style.display== DisplayStyle.Flex ?  DisplayStyle.None : DisplayStyle.Flex;
        _root.Q<Button>("btnBack").style.display = obj.style.display;
    }

    private void OnExit(ClickEvent evt)
    {
        Utilities.Quit();
    }

    private void OnJoin(ClickEvent evt)
    {
        _root.Q("mainMenu").style.display = DisplayStyle.None;
        _root.Q("join").style.display = DisplayStyle.Flex;
        _root.Q<Button>("btnBack").style.display = DisplayStyle.Flex;
    }
    
    void StartGame(ClickEvent evt)
    {
        _session.AsHost().IsLocked = true;
        _session.AsHost().IsPrivate = true;
        
        NetworkManager.Singleton.SceneManager.LoadScene("Game Setup", LoadSceneMode.Single);
        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(projectManagerPrefab);
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadComplete;
    }

    private void SceneManagerOnOnLoadComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnOnLoadComplete;
        
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
            
        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(gameSetupPrefab);
    }
}