using Unity.Cinemachine;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;

    public void SetCamera(Transform target)
    {
        cam.Target.TrackingTarget = target;
    }
}
