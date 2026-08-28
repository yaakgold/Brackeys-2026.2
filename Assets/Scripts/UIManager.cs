using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text hourText;
    [SerializeField] private Slider sanitySlider;
    [SerializeField] private EodPanelController eodPanelController;
    [SerializeField] private EndOfJamPanel eojPanelController;

    private PlayerMovement _player;
    
    public void OpenEndOfDayUI()
    {
        //TODO: Handle what to do if other players are in menus. I think I am going to force them out, like Among Us
        _player.GetComponent<PlayerInput>().enabled = false;
        eodPanelController.gameObject.SetActive(true);
    }

    public void CloseEndOfDayUI()
    {
        eodPanelController.gameObject.SetActive(false);
        _player.GetComponent<PlayerInput>().enabled = true;
    }

    public void OpenEndOfJamUI()
    {
        eojPanelController.gameObject.SetActive(true);
        _player.GetComponent<PlayerInput>().enabled = false;
    }

    public void SetDayText(int day)
    {
        dayText.text = $"Day: {day}";
    }

    public void SetHourText(int hour)
    {
        hourText.text = $"Hour: {hour}";
    }

    /// <summary>
    /// Set sanity
    /// </summary>
    /// <param name="sanity">New value / 100</param>
    public void SetSanity(int sanity)
    {
        sanitySlider.value = sanity;
    }

    public void OnNextDayButton()
    {
        ProjectManager.Instance.StartNextDayRpc();
    }

    public void SetPlayer(PlayerMovement playerMovement)
    {
        _player = playerMovement;
    }
}
