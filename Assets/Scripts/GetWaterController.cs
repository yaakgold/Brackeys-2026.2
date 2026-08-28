using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GetWaterController : Minigame
{
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private float updateSpeed;
    [SerializeField] private float width, height;

    private VisualElement _root;
    private IEnumerator _countUpCoroutine;
    
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
        
        _root.Q<Button>("btnFill").RegisterCallback<PointerDownEvent>(OnFillDown, TrickleDown.TrickleDown);
        _root.Q<Button>("btnFill").RegisterCallback<PointerUpEvent>(OnFillUp, TrickleDown.TrickleDown);
        
        var pgbCup = _root.Q<ProgressBar>("pgbCup"); 
        pgbCup.style.width = width;
        pgbCup.style.height = height;
    }

    private void SetSliderValue()
    {
        var pgbCup = _root.Q<ProgressBar>("pgbCup");
        pgbCup.value++;
    }

    private void OnFillDown(PointerDownEvent evt)
    {
        _countUpCoroutine = CountUp();
        StartCoroutine(_countUpCoroutine);
    }
    
    private void OnFillUp(PointerUpEvent evt)
    {
        StopCoroutine(_countUpCoroutine);
        
        onCompleteMinigame.Invoke(Mathf.CeilToInt(20 * (_root.Q<ProgressBar>("pgbCup").value / 100)));
    }

    private IEnumerator CountUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateSpeed);

            SetSliderValue();
        }
    }
}
