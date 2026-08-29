using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndOfJamPanel : MonoBehaviour
{
    [SerializeField] private PanelRenderer panelRenderer;

    private VisualElement _root;
    private MultiColumnListView _multiColumnListView;
    
    private void OnEnable()
    {
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _root = rootElement;

        var qualityType = EGameQualityType.Excellent;
        
        _root.Q<Label>("lblTotalQualityAdded").text = 
            $"You made a game with a quality level of: {ProjectManager.Instance.GetQuality()}\n" +
            $"This is considered a(n) {qualityType.ToString().ToLower()} game";
        
        _root.Q<Button>("btnLeave").RegisterCallback<ClickEvent>(e =>
        {
            MultiplayerService.Instance.Sessions["default-session"].LeaveAsync();
            SceneManager.LoadScene("Main Menu");
            Destroy(PausePanelController.Instance.gameObject);
        });
        
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
        
        _multiColumnListView = _root.Q<MultiColumnListView>("mclvEndResults");
        print(_multiColumnListView);
        if (_multiColumnListView == null) return;
        
        _multiColumnListView.itemsSource = players;
        _multiColumnListView.columns[0].makeCell = () => new Label();
        _multiColumnListView.columns[1].makeCell = () => new Label();
        _multiColumnListView.columns[2].makeCell = () => new Label();

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
            ((Label)element).text = ((PlayerEod)_multiColumnListView.itemsSource[i]).Role.ToString();
        };
    }
    
    public void OpenPanel()
    {
        _root.Q("pnlEndOfJam").style.display = DisplayStyle.Flex;
        
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

        _multiColumnListView.Rebuild();
    }
}
