using Unity.Cinemachine;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private Camera kickCamera;

    public void SwitchToKickCamera()
    {
        cam.gameObject.SetActive(false);
        kickCamera.gameObject.SetActive(true);
    }
    
    public void SetCamera(Transform target)
    {
        cam.Target.TrackingTarget = target;
    }
}
