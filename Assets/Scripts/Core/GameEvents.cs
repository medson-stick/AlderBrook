using UnityEngine.Events;

// Holds global events that other scripts can subscribe to.
public static class GameEvents
{
    public static UnityEvent<int> NewDay = new UnityEvent<int>();
}
