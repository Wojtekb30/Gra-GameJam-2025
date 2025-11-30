using UnityEngine;
using TMPro;
using System;

public class PlayerTime : MonoBehaviour
{
    [Tooltip("Maximum time in seconds (must be >= 1)")]
    public int maxTime = 100;

    [Tooltip("Start time in seconds; clamped to 0..maxTime")]
    public int startTime = 100;

    [Tooltip("Optional TextMeshProUGUI element to display time")]
    public TextMeshProUGUI timeText;

    // ---------- Persistent static fields ----------
    // int.MinValue works as a sentinel that tells us the timer has never been initialised.
    private static int  s_sharedMaxTime       = int.MinValue;
    private static int  s_sharedCurrentTime   = int.MinValue;
    private static float s_sharedSecondTimer   = 0f;
    // ----------------------------------------------

    // Event fired when time changes: (currentSeconds, maxSeconds)
    public event Action<int, int> OnTimeChanged;

    // Event fired when timer reaches zero
    public event Action OnTimeExpired;

    void Reset()
    {
        maxTime = 100;
        startTime = 100;
    }

    void Awake()
    {
        // First instance ever created – initialise the static values.
        if (s_sharedMaxTime == int.MinValue)
        {
            s_sharedMaxTime       = Mathf.Max(1, maxTime);
            s_sharedCurrentTime   = Mathf.Clamp(startTime, 0, s_sharedMaxTime);
            s_sharedSecondTimer   = 0f;
        }
        else
        {
            // Subsequent instances just adopt the already‑saved values.
            maxTime = s_sharedMaxTime;
        }
    }

    void Start()
    {
        // Ensure maxTime is never below 1 (in case it was changed in the inspector).
        if (maxTime < 1) maxTime = 1;

        // Keep the static values in sync if the inspector changed them before the first scene load.
        // Do **not** reset currentTime – we want the persisted value to survive scene changes.
        s_sharedMaxTime = maxTime;

        UpdateTimeUI();
    }

    void Update()
    {
        if (s_sharedCurrentTime <= 0) return;

        s_sharedSecondTimer += Time.deltaTime;
        while (s_sharedSecondTimer >= 1f && s_sharedCurrentTime > 0)
        {
            s_sharedSecondTimer -= 1f;
            s_sharedCurrentTime--;
            NotifyChanged();

            if (s_sharedCurrentTime <= 0)
            {
                s_sharedCurrentTime = 0;
                OnTimeExpired?.Invoke();
                break;
            }
        }
    }

    // Adds seconds to the timer, returns actual seconds added
    public int AddTime(int seconds)
    {
        if (seconds <= 0) return 0;
        int before = s_sharedCurrentTime;
        s_sharedCurrentTime = Mathf.Clamp(s_sharedCurrentTime + seconds, 0, s_sharedMaxTime);
        int added = s_sharedCurrentTime - before;
        if (added > 0) NotifyChanged();
        return added;
    }

    public int SubtractTime(int seconds)
    {
        if (seconds <= 0) return 0;
        int before = s_sharedCurrentTime;
        s_sharedCurrentTime = Mathf.Clamp(s_sharedCurrentTime - seconds, 0, s_sharedMaxTime);
        int subtracted = before - s_sharedCurrentTime;
        if (subtracted > 0) NotifyChanged();
        if (s_sharedCurrentTime == 0) OnTimeExpired?.Invoke();
        return subtracted;
    }

    public void SetMaxTime(int newMax, bool preservePercentage = true)
    {
        if (newMax < 1) newMax = 1;

        if (preservePercentage)
        {
            float pct = (float)s_sharedCurrentTime / s_sharedMaxTime;
            s_sharedMaxTime = newMax;
            s_sharedCurrentTime = Mathf.Clamp(Mathf.RoundToInt(pct * s_sharedMaxTime), 0, s_sharedMaxTime);
        }
        else
        {
            s_sharedMaxTime = newMax;
            s_sharedCurrentTime = Mathf.Clamp(s_sharedCurrentTime, 0, s_sharedMaxTime);
        }

        NotifyChanged();
    }

    public int GetCurrentTime() => s_sharedCurrentTime;
    public int GetMaxTime() => s_sharedMaxTime;

    void NotifyChanged()
    {
        UpdateTimeUI();
        OnTimeChanged?.Invoke(s_sharedCurrentTime, s_sharedMaxTime);
    }

    void UpdateTimeUI()
    {
        if (timeText != null)
            timeText.text = $"Time: {s_sharedCurrentTime}/{s_sharedMaxTime}";
    }
}
