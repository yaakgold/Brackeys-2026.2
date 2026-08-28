using UnityEngine;
using UnityEngine.Events;

public class Minigame : MonoBehaviour
{
    /// <summary>
    /// The value will be the quality.
    /// </summary>
    public UnityEvent<int> onCompleteMinigame;

    public bool requiresRenderer;
    public Material minigameMaterial;

    public virtual void StartMinigame()
    {
    }
    
    public virtual void StopMinigame()
    {}
}