using UnityEngine;

// Lets the player sleep and start the next day.
public class Bed : MonoBehaviour, IInteractive
{
    // Starts a new day when the player interacts.
    public void OnInteract()
    {
        if (DayManager.Instance == null) return;
        DayManager.Instance.StartNewDay();
    }
}
