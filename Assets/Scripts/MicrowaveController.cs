using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MicrowaveController : Minigame
{
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private int minTime, maxTime;
    [SerializeField] private float updateSpeed;

    private VisualElement _root;
    private IEnumerator _countUpCoroutine;
    
    private int _currentSeconds;
    private int _secondsToReach = 100;
    
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
        
        _root.Q<Button>("btnCook").RegisterCallback<PointerDownEvent>(OnCookDown, TrickleDown.TrickleDown);
        _root.Q<Button>("btnCook").RegisterCallback<PointerUpEvent>(OnCookUp, TrickleDown.TrickleDown);

        
        _secondsToReach = Random.Range(minTime, maxTime);
        var minutes = Mathf.Floor(_secondsToReach / 60.0f);
        var seconds = _secondsToReach - (minutes * 60.0f);

        _root.Q<Label>("lblRequestedCookTime").text = $"Cook until: {minutes:00}:{seconds:00}";
    }

    private void SetSliderValue()
    {
        _root.Q<ProgressBar>("pgbCookTime").SetValueWithoutNotify((_currentSeconds / (float)_secondsToReach) * 100);

        var minutes = Mathf.Floor(_currentSeconds / 60.0f);
        var seconds = _currentSeconds - (minutes * 60.0f);
        
        _root.Q<ProgressBar>("pgbCookTime").title = $"{minutes:00}:{seconds:00}";
    }

    private void OnCookDown(PointerDownEvent evt)
    {
        _countUpCoroutine = CountUp();
        StartCoroutine(_countUpCoroutine);
        AudioManager.Instance.Play("Microwave");
    }

    public override void StopMinigame()
    {
        base.StopMinigame();
        
        AudioManager.Instance.Stop("Microwave");

        StopCoroutine(_countUpCoroutine);

        var sanityToAdd = 20 - Mathf.Abs(_secondsToReach - _currentSeconds);
        sanityToAdd = Mathf.Clamp(sanityToAdd, -10, 20);
        
        onCompleteMinigame.Invoke(sanityToAdd);
    }

    private void OnCookUp(PointerUpEvent evt)
    {
        StopMinigame();
    }

    private IEnumerator CountUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateSpeed);

            _currentSeconds++;
            SetSliderValue();
        }
    }
}
