using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager Instance { get; private set; }

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

    [SerializeField] private GameObject endOfDayUI;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text hourText;
    [SerializeField] private Slider sanitySlider;
    
    public void OpenEndOfDayUI()
    {
        //TODO: Handle what to do if other players are in menus. I think I am going to force them out, like Among Us
        endOfDayUI.SetActive(true);
    }

    public void SetDayText(int day)
    {
        dayText.text = $"Day: {day}";
    }

    public void SetHourText(int hour)
    {
        hourText.text = $"Hour: {hour}";
    }

    public void SetSanity(int sanity)
    {
        sanitySlider.value = sanity;
    }
}
