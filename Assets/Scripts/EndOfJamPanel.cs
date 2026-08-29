using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EndOfJamPanel : MonoBehaviour
{
    [SerializeField] private PanelRenderer panelRenderer;

    private VisualElement _root;
    private MultiColumnListView _multiColumnListView;
    private int _totalQuality;
    private EGameQualityType _qualityType;
    
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
        
        _root.Q<Button>("btnLeave").RegisterCallback<ClickEvent>(e =>
        {
            Utilities.LeaveSession();
        });
        
        _multiColumnListView = _root.Q<MultiColumnListView>("mclvEndResults");
        if (_multiColumnListView == null) return;
        
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
            ((Label)element).AddToClassList("blocks-label");
        };
    }
    
    public void OpenPanel()
    {
        _root.Q("pnlEndOfJam").style.display = DisplayStyle.Flex;
        
        _totalQuality = 0;
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
            
            _totalQuality += player.Value;
            print(_totalQuality + " Other");
        }

        //TODO: Is this good values????
        _qualityType = _totalQuality switch
        {
            > 30 => EGameQualityType.Perfect,
            > 25 => EGameQualityType.Excellent,
            > 20 => EGameQualityType.Great,
            > 15 => EGameQualityType.Good,
            > 10 => EGameQualityType.Average,
            > 5  => EGameQualityType.Mediocre,
            _ => EGameQualityType.Terrible
        };
        
        _root.Q<Label>("lblTotalQualityAdded").text = 
            $"You made a game with a quality level of: {ProjectManager.Instance.GetQuality()}\n" +
            $"This is considered a(n) {_qualityType.ToString().ToLower()} game";
        
        _multiColumnListView.itemsSource = players;

        _multiColumnListView.Rebuild();
    }
}
