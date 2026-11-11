using UnityEngine;
using UnityEngine.Events;

// Stores and manages the player's money.
public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    [SerializeField] private int startingMoney = 500;
    public UnityEvent<int> onMoneyChanged = new UnityEvent<int>();

    public int Money { get; private set; }

    // Keeps only one wallet alive and sets the starting money.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Money = startingMoney;
        onMoneyChanged.Invoke(Money);
    }

    // Spends money if the player can afford it.
    public bool SpendMoney(int amount)
    {
        if (amount <= 0 || Money < amount) return false;
        Money -= amount;
        onMoneyChanged.Invoke(Money);
        return true;
    }

    // Adds money to the wallet.
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        Money += amount;
        onMoneyChanged.Invoke(Money);
    }
}
