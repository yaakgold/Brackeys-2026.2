using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameSetupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text descriptionText;

    private const string DevRoleText = "YOU ARE A DEVELOPER";
    private const string BadRoleText = "YOU ARE BAD GUY";
    private const string DevDescText = "Make a good game (instructions to come)";
    private const string BadDescText = "Make a bad game (instructions to come)";

    private void Start()
    {
        if (!NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()
                .TryGetComponent(out MpPlayerController player))
        {
            print("Found nothing");
            return;
        }
        
        player.onPlayerRoleChanged.AddListener(role =>
        {
            switch (role)
            {
                case EPlayerRole.Dev:
                    roleText.text = DevRoleText;
                    descriptionText.text = DevDescText;
                    break;
                case EPlayerRole.BadGuy:
                    roleText.text = BadRoleText;
                    descriptionText.text = BadDescText;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });
        
        switch (player.GetRole())
        {
            case EPlayerRole.Dev:
                roleText.text = DevRoleText;
                descriptionText.text = DevDescText;
                break;
            case EPlayerRole.BadGuy:
                roleText.text = BadRoleText;
                descriptionText.text = BadDescText;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
