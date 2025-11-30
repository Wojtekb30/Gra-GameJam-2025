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

    // Current time in whole seconds
    private int currentTime;

    // Event fired when time changes: (currentSeconds, maxSeconds)
    public event Action<int,int> OnTimeChanged;

    // Event fired when timer reaches zero
    public event Action OnTimeExpired;

    // Internal accumulator to track fractional seconds
    private float secondTimer = 0f;

    void Reset()
    {
        maxTime = 100;
        startTime = 100;
    }

    void Start()
    {
        if (maxTime < 1) maxTime = 1;
        currentTime = Mathf.Clamp(startTime, 0, maxTime);
        UpdateTimeUI();
    }

    void Update()
    {
        if (currentTime <= 0) return;

        secondTimer += Time.deltaTime;
        while (secondTimer >= 1f && currentTime > 0)
        {
            secondTimer -= 1f;
            currentTime--;
            NotifyChanged();

            if (currentTime <= 0)
            {
                currentTime = 0;
                OnTimeExpired?.Invoke();
                break;
            }
        }
    }

    // Adds seconds to the timer, returns actual seconds added
    public int AddTime(int seconds)
    {
        if (seconds <= 0) return 0;
        int before = currentTime;
        currentTime = Mathf.Clamp(currentTime + seconds, 0, maxTime);
        int added = currentTime - before;
        if (added > 0) NotifyChanged();
        return added;
    }
    public int SubtractTime(int seconds)
    {
        if (seconds <= 0) return 0;
        int before = currentTime;
        currentTime = Mathf.Clamp(currentTime - seconds, 0, maxTime);
        int subtracted = before - currentTime;
         if (subtracted > 0) NotifyChanged();
        if (currentTime == 0) OnTimeExpired?.Invoke();
      return subtracted;
    }


    public void SetMaxTime(int newMax, bool preservePercentage = true)
    {
        if (newMax < 1) newMax = 1;

        if (preservePercentage)
        {
            float pct = (float)currentTime / maxTime;
            maxTime = newMax;
            currentTime = Mathf.Clamp(Mathf.RoundToInt(pct * maxTime), 0, maxTime);
        }
        else
        {
            maxTime = newMax;
            currentTime = Mathf.Clamp(currentTime, 0, maxTime);
        }

        NotifyChanged();
    }

    public int GetCurrentTime() => currentTime;
    public int GetMaxTime() => maxTime;

    void NotifyChanged()
    {
        UpdateTimeUI();
        OnTimeChanged?.Invoke(currentTime, maxTime);
    }

    void UpdateTimeUI()
    {
        if (timeText != null)
            timeText.text = $"Time: {currentTime}/{maxTime}";
    }
}
