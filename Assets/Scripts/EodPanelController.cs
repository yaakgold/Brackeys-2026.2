using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class PlayerEod
{
    public ulong PlayerId { get; set; }
    public string PlayerName { get; set; }
    public EPlayerRole Role { get; set; }
    public int QualityAdded { get; set; }
}

public class EodPanelController : NetworkBehaviour
{
    [SerializeField] private PanelRenderer panelRenderer;

    private VisualElement _root;
    private MultiColumnListView _multiColumnListView;
    

    private List<PlayerEod> _players;
    
    public bool canVote = true;

    private Dictionary<ulong, int> _playerVotes;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            //NetworkObject.Spawn();
        }
    }

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

        _root.Q<Label>("lblTitle").text = $"End Of Day {ProjectManager.Instance.GetDay()}";
        
        _playerVotes = new Dictionary<ulong, int>();
        _players = new List<PlayerEod>();
        foreach (var player in ProjectManager.Instance.DictTaskLog)
        {
            var mpPlayer = GameSetup.Instance.GetPlayerController(player.Key);
            _players.Add(new PlayerEod()
            {
                PlayerId = player.Key,
                PlayerName = mpPlayer.GetName(),
                Role = mpPlayer.GetRole(),
                QualityAdded = player.Value
            });
        }
        
        var dataBinding = new DataBinding
        {
            bindingMode = BindingMode.ToTarget,
            dataSource = this,
            dataSourcePath = new PropertyPath(nameof(canVote)),
        };
        
        _root.Q<Button>("btnSkip").dataSource = canVote;
        _root.Q<Button>("btnSkip").SetBinding("enabledSelf", dataBinding);
        _root.Q<Button>("btnSkip").RegisterCallback<ClickEvent>(evt => OnVoteClicked(
            new PlayerEod
            {
                PlayerId = 9999
            }));

        
        _multiColumnListView = _root.Q<MultiColumnListView>("mcListView");
        _multiColumnListView.itemsSource = _players;
        _multiColumnListView.columns[0].makeCell = () => new Label();
        _multiColumnListView.columns[1].makeCell = () => new Label();
        _multiColumnListView.columns[2].makeCell = () => new Button();

        //Player name
        _multiColumnListView.columns[0].bindCell = (element, i) =>
        {
            ((Label)element).text = ((PlayerEod)_multiColumnListView.itemsSource[i]).PlayerName;
            ((Label)element).AddToClassList("blocks-label");
        };
        
        //Total Quality Added
        _multiColumnListView.columns[1].bindCell = (element, i) =>
        {
            ((Label)element).text = $"Total Quality Added: {((PlayerEod)_multiColumnListView.itemsSource[i]).QualityAdded}";
            ((Label)element).AddToClassList("blocks-label");
        };
        
        //Vote button
        _multiColumnListView.columns[2].bindCell = (element, i) =>
        {
            ((Button)element).text = "Vote";
            ((Button)element).AddToClassList("blocks-button");
            ((Button)element).RegisterCallback<ClickEvent>(evt => OnVoteClicked((PlayerEod)_multiColumnListView.itemsSource[i]));

            ((Button)element).dataSource = canVote;
            ((Button)element).SetBinding("enabledSelf", dataBinding);
        };
    }

    private void OnVoteClicked([CanBeNull] PlayerEod player)
    {
        canVote = false;
        PlayerVotedServerRpc(player?.PlayerId ?? 9999);
    }

    public void OpenPanel()
    {
        _playerVotes = new Dictionary<ulong, int>();
        _root.Q("pnlEod").style.display = DisplayStyle.Flex;
        
        _root.Q<Label>("lblTitle").text = $"End Of Day {ProjectManager.Instance.GetDay()}";
        
        var players = new List<PlayerEod>();
        foreach (var player in ProjectManager.Instance.DictTaskLog)
        {
            var mpPlayer = GameSetup.Instance.GetPlayerController(player.Key);
            players.Add(new PlayerEod()
            {
                PlayerId = player.Key,
                PlayerName = mpPlayer.GetName(),
                Role = mpPlayer.GetRole(),
                QualityAdded = player.Value
            });
        }
        _multiColumnListView.itemsSource = players;
        _players = players;

        _multiColumnListView.Rebuild();
    }
    
    public void ClosePanel()
    {
        _root.Q("pnlEod").style.display = DisplayStyle.None;
    }

    [Rpc(SendTo.Server)]
    private void PlayerVotedServerRpc(ulong votedId)
    {
        if (_playerVotes.TryGetValue(votedId, out var taskLog))
        {
            _playerVotes[votedId]++;
        }
        else
        {
            _playerVotes[votedId] = 1;
        }

        var votes = 0;
        foreach (var playerVote in _playerVotes)
        {
            votes += playerVote.Value;
        }
        
        //Close panel
        if (votes == _players.Count)
        {
            var player = _playerVotes.FirstOrDefault(pair => pair.Value >= Mathf.CeilToInt(_players.Count * .5f) 
                                                             && pair.Key != 9999);
            
            //This means there was a majority on someone
            if (player is not { Key: 0, Value: 0 })
            {
                //TODO: Handle a player being kicked off the team
            }
            canVote = true;
            PlayerVotedClientRpc();
            UIManager.Instance.CloseEndOfDayUI();
        } 

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayerVotedClientRpc()
    {
        canVote = true;
        UIManager.Instance.CloseEndOfDayUI();
    }
}
