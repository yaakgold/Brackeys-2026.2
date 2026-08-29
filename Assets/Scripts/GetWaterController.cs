using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GetWaterController : Minigame
{
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private float waterUpdateSpeed;
    [SerializeField] private float cupChangeSpeed;
    [SerializeField] private Texture[] waterTextures;
    [SerializeField] private Texture[] cupTextures;

    private VisualElement _root;
    private IEnumerator _waterChangeCoroutine;
    private IEnumerator _cupChangeCoroutine;
    private Image _waterImage;
    private Image _cupImage;
    
    private int _currentWaterIndex;
    private int _currentCupIndex;
    
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

        _waterImage = _root.Q<Image>("imgWater");
        _cupImage = _root.Q<Image>("imgCup");
    }
    private void OnFillDown(PointerDownEvent evt)
    {
        AudioManager.Instance.Play("Water Pour");
        _waterChangeCoroutine = WaterChangeCo();
        StartCoroutine(_waterChangeCoroutine);
        
        _cupChangeCoroutine = CupChangeCo();
        StartCoroutine(_cupChangeCoroutine);
    }
    
    private void OnFillUp(PointerUpEvent evt)
    {
        StopMinigame();
    }

    public override void StopMinigame()
    {
        base.StopMinigame();
        
        AudioManager.Instance.Stop("Water Pour");
        
        StopCoroutine(_waterChangeCoroutine);
        StopCoroutine(_cupChangeCoroutine);
        
        onCompleteMinigame.Invoke(1 + _currentCupIndex);
    }

    private IEnumerator WaterChangeCo()
    {
        while (true)
        {
            _waterImage.image = waterTextures[_currentWaterIndex];
            _currentWaterIndex = (_currentWaterIndex + 1) % waterTextures.Length;
            yield return new WaitForSeconds(waterUpdateSpeed);
        }
    }
    
    private IEnumerator CupChangeCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(cupChangeSpeed);
            _cupImage.image = cupTextures[_currentCupIndex];
            _currentCupIndex++;
            
            if (_currentCupIndex >= cupTextures.Length)
                StopMinigame();
        }
    }
}
