using TMPro;
using UnityEngine;

// Updates the day, time, and money text on the HUD.
public class HUDController : MonoBehaviour
{
    [Header("HUD Text")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text moneyText;

    // Subscribes to wallet changes and shows the starting money.
    private void Start()
    {
        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.onMoneyChanged.AddListener(UpdateMoney);
            UpdateMoney(PlayerWallet.Instance.Money);
        }
    }

    // Removes the wallet listener when the HUD is destroyed.
    private void OnDestroy()
    {
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.onMoneyChanged.RemoveListener(UpdateMoney);
    }

    // Refreshes the day and time every frame.
    private void Update()
    {
        if (DayManager.Instance == null) return;
        if (dayText != null) dayText.text = $"Day {DayManager.Instance.CurrentDay}";
        if (timeText != null) timeText.text = DayManager.Instance.GetTimeString();
    }

    // Updates the money label.
    private void UpdateMoney(int amount)
    {
        if (moneyText != null) moneyText.text = $"{amount}g";
    }
}
