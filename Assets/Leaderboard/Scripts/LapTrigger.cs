using UnityEngine;

namespace Leaderboard.Scripts
{
    /// <summary>
    /// Handles the lap triggers.
    /// </summary>
    public class LapTrigger : MonoBehaviour
    {
        public enum TriggerType
        {
            LapStarter,
            LapEnder
        }
    
        public TriggerType triggerType = TriggerType.LapStarter;
    
        /// <summary>
        /// Handles the trigger event.
        /// </summary>
        /// <param name="other">The object that is collided with</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                switch (triggerType)
                {
                    case TriggerType.LapStarter:
                        if (LeaderboardSaveData.Instance.inLap)
                        {
                            return;
                        }
                    
                        LeaderboardSaveData.Instance.canIncrementTime = true;
                        LeaderboardSaveData.Instance.inLap = true;
                        break;
                    case TriggerType.LapEnder:
                        if (!LeaderboardSaveData.Instance.inLap)
                        {
                            return;
                        }
                    
                        LeaderboardSaveData.Instance.canIncrementTime = false;
                        LeaderboardSaveData.Instance.inLap = false;
                    
                        LeaderboardSaveData.Instance.OpenNameMenu();
                        break;
                }
            
                gameObject.SetActive(false);
            }
        }
    }
}
