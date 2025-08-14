using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorMinigameManager : MonoBehaviour
{
    [Header("Minigame Settings")]
    [Tooltip("List of possible minigame scenes for this door.")]
    [SerializeField] private List<string> minigameScenes;

    [Tooltip("Connected door to open after completion.")]
    [SerializeField] private Door connectedDoor;

    private GameObject player;

    private void Start()
    {
        if (minigameScenes == null || minigameScenes.Count == 0)
        {
            Debug.LogWarning($"⚠ No minigame scenes set for {gameObject.name}");
        }
    }

    public void SetPlayer(GameObject playerObj)
    {
        player = playerObj;
    }

    public void StartMinigame()
    {
        if (player == null)
        {
            Debug.LogError("❌ Player reference missing. Cannot start minigame.");
            return;
        }

        // Save return position
        MinigameState.ReturnPosition = player.transform.position;
        Debug.Log($"📌 Saved return position: {MinigameState.ReturnPosition}");

        // Save game state before loading minigame
        FindObjectOfType<SaveController>()?.SaveGame();

        // Pick random scene
        string chosenScene = GetRandomScene();
        if (string.IsNullOrEmpty(chosenScene))
        {
            Debug.LogError("❌ No minigame scene chosen.");
            return;
        }

        Debug.Log($"🎮 Loading Minigame Scene: {chosenScene}");
        SceneManager.LoadScene(chosenScene);
    }

    private string GetRandomScene()
    {
        if (minigameScenes == null || minigameScenes.Count == 0) return null;
        return minigameScenes[Random.Range(0, minigameScenes.Count)];
    }

    public void OnMinigameCompleted(bool won)
    {
        MinigameState.MinigameCompleted = true;
        MinigameState.DoorShouldBeOpen = won;

        if (won && connectedDoor != null)
        {
            connectedDoor.OpenDoor();
        }

        // Save minigame result
        FindObjectOfType<SaveController2>()?.SaveGame();

        // Return to main level
        SceneManager.LoadScene("Level 1");
    }
}

