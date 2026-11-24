using TMPro;
using UnityEngine;

// Updates the day, time, and money text on the HUD only when values change.
public class HUDController : MonoBehaviour
{
    [Header("HUD Text")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text moneyText;

    // Subscribes to wallet and clock changes.
    private void Start()
    {
        if (PlayerWallet.Instance != null)
        {
            PlayerWallet.Instance.onMoneyChanged.AddListener(UpdateMoney);
            UpdateMoney(PlayerWallet.Instance.Money);
        }

        GameEvents.TimeChanged.AddListener(UpdateTime);
        UpdateTime();
    }

    // Removes the listeners when the HUD is destroyed.
    private void OnDestroy()
    {
        if (PlayerWallet.Instance != null) PlayerWallet.Instance.onMoneyChanged.RemoveListener(UpdateMoney);
        GameEvents.TimeChanged.RemoveListener(UpdateTime);
    }

    // Updates the day and time labels.
    private void UpdateTime()
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
