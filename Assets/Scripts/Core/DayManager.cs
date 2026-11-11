using System.Collections;
using UnityEngine;

// Tracks the current day and time.
public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Day Settings")]
    [SerializeField] private int startingDay = 1;
    [SerializeField] private int dayStartHour = 6;
    [SerializeField] private int dayEndHour = 26;
    [SerializeField] private float secondsPerGameMinute = 1f;

    [Header("Day Transition")]
    [SerializeField] private float fadeDuration = 0.5f;

    public int CurrentDay { get; private set; }
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }

    private float minuteTimer;
    private ScreenFader screenFader;
    private PlayerController playerController;
    private bool isChangingDay;

    // Keeps only one DayManager alive and caches the fade and player.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        screenFader = FindFirstObjectByType<ScreenFader>();
        if (screenFader == null) screenFader = gameObject.AddComponent<ScreenFader>();

        playerController = FindFirstObjectByType<PlayerController>();
    }

    // Sets the starting day and time.
    private void Start()
    {
        CurrentDay = startingDay;
        CurrentHour = dayStartHour;
        CurrentMinute = 0;
    }

    // Advances the clock as real time passes.
    private void Update()
    {
        if (secondsPerGameMinute <= 0f) return;

        minuteTimer += Time.deltaTime;
        if (minuteTimer < secondsPerGameMinute) return;

        minuteTimer -= secondsPerGameMinute;
        AddMinute();
    }

    // Adds one minute and rolls over hours and days.
    private void AddMinute()
    {
        CurrentMinute++;
        if (CurrentMinute >= 60)
        {
            CurrentMinute = 0;
            CurrentHour++;
        }

        if (CurrentHour >= dayEndHour) StartNewDay();
    }

    // Starts the fade-out, advances the day, then fades back in.
    public void StartNewDay()
    {
        if (isChangingDay) return;
        StartCoroutine(NewDayRoutine());
    }

    // Handles the full day transition.
    private IEnumerator NewDayRoutine()
    {
        isChangingDay = true;
        if (playerController != null) playerController.SetInputEnabled(false);

        if (screenFader != null) yield return screenFader.FadeOut(fadeDuration);

        CurrentDay++;
        CurrentHour = dayStartHour;
        CurrentMinute = 0;
        minuteTimer = 0f;
        GameEvents.NewDay.Invoke(CurrentDay);

        if (screenFader != null) yield return screenFader.FadeIn(fadeDuration);
        if (playerController != null) playerController.SetInputEnabled(true);

        isChangingDay = false;
    }

    // Returns the current time as a 12-hour clock string.
    public string GetTimeString()
    {
        int displayHour = CurrentHour % 24;
        int hour12 = displayHour % 12;
        if (hour12 == 0) hour12 = 12;
        string suffix = displayHour < 12 ? "AM" : "PM";
        return $"{hour12}:{CurrentMinute:00} {suffix}";
    }
}
