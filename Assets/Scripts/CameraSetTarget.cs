using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraSetTarget : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;

    private void Start()
    {
        var target = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        cam.Target.TrackingTarget = target.transform;
    }
}
