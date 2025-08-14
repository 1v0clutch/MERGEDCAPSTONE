// TimerManager.cs - Handles all timer-related functionality
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public TMP_Text timerText;
    public float timeLimit = 30f;

    private float currentTime;
    private bool isTimerModeActive = false;

    public bool IsTimerModeActive => isTimerModeActive;

    public void StartTimerMode()
    {
        isTimerModeActive = true;
        currentTime = timeLimit;
        timerText.gameObject.SetActive(true);
    }

    public void StopTimerMode()
    {
        isTimerModeActive = false;
        timerText.gameObject.SetActive(false);
    }

    public void UpdateTimer()
    {
        if (!isTimerModeActive) return;

        currentTime -= Time.deltaTime;
        timerText.text = $"TIME: {Mathf.CeilToInt(currentTime)}";
    }

    public bool HasTimeExpired()
    {
        return currentTime <= 0;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
}