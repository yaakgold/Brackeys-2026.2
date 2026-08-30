using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameSetupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text descriptionText;

    private const string EndingText = "\n\nRemember the lower your sanity level, the more difficult it will be to do good quality work, and the slower you will move.\nRemember, you have 3 days until the end of the jam.";

    private const string StartingText =
        "Welcome to the Brackeys 2026.2.5 Game Jam! This is a 3 day long jam where you and your friends are trying to make the best game you possibly can, while your sanity takes a tole";
    private const string DevRoleText = "YOU ARE A DEVELOPER";
    private const string BadRoleText = "YOU ARE A VIBE CODER";

    private const string DevDescText =
        "\n\nMake the best game you can.\nThere is a Vibe Coder on the team.\nThey will try to make development more difficult and delete work from your computers.";

    private const string BadDescText =
        "\n\nDelet\u0065 Work from the other developers computers to lower the quality of the game.\nMake sure to not get caught with low quality on the End Of Day Reports.";

    private void Start()
    {
        if (!NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()
                .TryGetComponent(out MpPlayerController player))
        {
            return;
        }
        
        player.onPlayerRoleChanged.AddListener(role =>
        {
            switch (role)
            {
                case EPlayerRole.Developer:
                    roleText.text = DevRoleText;
                    descriptionText.text = StartingText + DevDescText + EndingText;
                    break;
                case EPlayerRole.VideCoder:
                    roleText.text = BadRoleText;
                    descriptionText.text = StartingText + BadDescText + EndingText;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });
        
        switch (player.GetRole())
        {
            case EPlayerRole.Developer:
                roleText.text = DevRoleText;
                descriptionText.text = StartingText + DevDescText + EndingText;
                break;
            case EPlayerRole.VideCoder:
                roleText.text = BadRoleText;
                descriptionText.text = StartingText + BadDescText + EndingText;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
