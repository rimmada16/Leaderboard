using UnityEngine;

namespace Leaderboard.Scripts
{
    /// <summary>
    /// Handles the cursor state
    /// </summary>
    public class CursorHandler : MonoBehaviour
    {
        public static CursorHandler Instance;
    
        /// <summary>
        /// Sets the instance of the CursorHandler.
        /// </summary>
        private void Awake() 
        { 
            // If there is an instance, and it's not me, delete myself.
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            }
        }
    
        /// <summary>
        /// Updates the cursor state.
        /// </summary>
        /// <param name="locked">The cursor state</param>
        public void SetCursorState(bool locked)
        {
            // If you have a GameManager script that handles the cursor state, you can remove this script.
            // Make sure to update the call in LeaderboardSaveData if you do!
        
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
