using UnityEngine;

public class SanityController : MonoBehaviour
{
    private int _sanityLevel = 100;
    
    public void UpdateSanity(int amount)
    {
        _sanityLevel = Mathf.Clamp(amount, 0, 100);

        UIManager.Instance.SetSanity(_sanityLevel);
    }
}