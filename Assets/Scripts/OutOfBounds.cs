using UnityEngine;

/// <summary>
/// Handles when the player enters the out of bounds area.
/// </summary>
public class OutOfBounds : MonoBehaviour
{
    /// <summary>
    /// Triggers when the player enters the out of bounds area.
    /// </summary>
    /// <param name="other">The collider that is collided with</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DemoGameManager.Instance == null)
            {
                Debug.LogError("DemoGameManager is null");
                return;
            }
            
            DemoGameManager.Instance.ResetLevel();
        }
    }
}
