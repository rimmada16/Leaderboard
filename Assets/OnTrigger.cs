using UnityEngine;

public class OnTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        LapStarter,
        LapEnder
    }
    
    public TriggerType triggerType = TriggerType.LapStarter;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (triggerType)
            {
                case TriggerType.LapStarter:
                    SaveTime.Instance.canIncrementTime = true;
                    break;
                case TriggerType.LapEnder:
                    SaveTime.Instance.canIncrementTime = false;
                    Debug.Log("Wouldve called");
                    SaveTime.Instance.OpenNameMenu();
                    break;
            }
            
            Debug.Log("Triggered");
            gameObject.SetActive(false);
        }
    }
}
