using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;
    public GameObject gameOverPanel;
    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("❌ GameOverPanel not assigned in GameOverManager!");
        }
    }

    public void RetryFromSave()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("⚠️ No save file found. Starting fresh instead.");
            ResetAllProgress(); // fallback
            return;
        }

        Time.timeScale = 1f; // Unpause the game
        SceneManager.LoadScene("Level 1"); // or your saved level scene name
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Unpause the game
        SceneManager.LoadScene("Main Menu"); // Make sure you have a scene named MainMenu
    }
    public void ResetAllProgress()
    {
        // Clear the save file
        string savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("🗑️ Save file deleted.");
        }

        // Reset MinigameState (optional)
        MinigameState.MinigameCompleted = false;
        MinigameState.DoorShouldBeOpen = false;
        MinigameState.ReturnPosition = Vector3.zero;

        // Optionally, reset other static game states or singleton data here

        // Reload from clean state
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1"); // or load your start scene
    }
}