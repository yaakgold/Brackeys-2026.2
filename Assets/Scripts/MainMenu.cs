using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private NetworkObject projectManagerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        startHostButton.onClick.AddListener(StartHost);
        startClientButton.onClick.AddListener(StartClient);
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        DeactivateButtons();
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Main Game", LoadSceneMode.Single);
        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(projectManagerPrefab);
        DeactivateButtons();
    }

    void DeactivateButtons()
    {
        startHostButton.interactable = false;
        startClientButton.interactable = false;
    }
}