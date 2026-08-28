using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class PlayerEod
{
    public ulong PlayerId { get; set; }
    public string PlayerName { get; set; }
    public int QualityAdded { get; set; }
}

public class EodPanelController : NetworkBehaviour
{
    [SerializeField] private PanelRenderer panelRenderer;

    private VisualElement _root;
    private ListView _playerListView;

    private List<PlayerEod> _players;
    
    public bool canVote = true;

    private Dictionary<ulong, int> _playerVotes;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            NetworkObject.Spawn();
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

        _playerVotes = new Dictionary<ulong, int>();
        _players = new List<PlayerEod>();
        foreach (var player in ProjectManager.Instance.DictTaskLog)
        {
            _players.Add(new PlayerEod()
            {
                PlayerId = player.Key,
                PlayerName = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(player.Key).gameObject.name,
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

        
        var multiColumnListView = _root.Q<MultiColumnListView>("mcListView");
        multiColumnListView.itemsSource = _players;
        multiColumnListView.columns[0].makeCell = () => new Label();
        multiColumnListView.columns[1].makeCell = () => new Label();
        multiColumnListView.columns[2].makeCell = () => new Button();

        //Player name
        multiColumnListView.columns[0].bindCell = (element, i) =>
        {
            ((Label)element).text = _players[i].PlayerName;
            ((Label)element).AddToClassList("blocks-label");
        };
        
        //Total Quality Added
        multiColumnListView.columns[1].bindCell = (element, i) =>
        {
            ((Label)element).text = $"Total Quality Added: {_players[i].QualityAdded}";
            ((Label)element).AddToClassList("blocks-label");
        };
        
        //Vote button
        multiColumnListView.columns[2].bindCell = (element, i) =>
        {
            ((Button)element).text = "Vote";
            ((Button)element).AddToClassList("blocks-button");
            ((Button)element).RegisterCallback<ClickEvent>(evt => OnVoteClicked(_players[i]));

            ((Button)element).dataSource = canVote;
            ((Button)element).SetBinding("enabledSelf", dataBinding);
        };
    }

    private void OnVoteClicked([CanBeNull] PlayerEod player)
    {
        canVote = false;
        PlayerVotedServerRpc(player?.PlayerId ?? 9999);
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

        //Close panel
        if (_playerVotes.Count == _players.Count)
        {
            var player = _playerVotes.FirstOrDefault(pair => pair.Value >= Mathf.CeilToInt(_players.Count * .5f) 
                                                             && pair.Key != 9999);
            
            //This means there was a majority on someone
            if (player is not { Key: 0, Value: 0 })
            {
                //TODO: Handle a player being kicked off the team
            }
        }
        
        UIManager.Instance.CloseEndOfDayUI();
    }
}
