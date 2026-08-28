using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EodPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private Button btnVoteSkip;
    //TODO: Setup skip ^

    public void Setup()
    {
        txtTitle.text = "End of day " + ProjectManager.Instance.GetDay();
        btnVoteSkip.onClick.AddListener(() =>
            {
                UIManager.Instance.OnNextDayButton();
            }
        );
    }
}
