using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorMinigameManager : MonoBehaviour
{
    [Header("Minigame Settings")]
    [Tooltip("List of possible minigame scenes for this door.")]
    [SerializeField] private List<string> minigameScenes;

    [Tooltip("All doors that this minigame can open.")]
    [SerializeField] private List<Door> connectedDoors;

    private GameObject player;
    private Door mainDoor; // The specific door linked to this minigame

    private void Awake()
    {
        if (connectedDoors == null || connectedDoors.Count == 0)
        {
            Debug.LogError($"❌ {name} has no connected doors assigned!");
            return;
        }

        // The "main" door for this minigame will be the first in the list
        mainDoor = connectedDoors[0];
    }

    private void Start()
    {
        if (mainDoor == null) return;

        string doorID = mainDoor.DoorID;

        // Restore state for THIS door only
        if (MinigameState.CompletedDoors.Contains(doorID))
        {
            OpenDoorByID(doorID);
        }
        else if (!string.IsNullOrEmpty(MinigameState.CurrentDoorID) &&
                 MinigameState.CurrentDoorID == doorID &&
                 MinigameState.MinigameCompleted &&
                 MinigameState.DoorShouldBeOpen)
        {
            OpenDoorByID(doorID);
            if (!MinigameState.CompletedDoors.Contains(doorID))
                MinigameState.CompletedDoors.Add(doorID);
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
        if (mainDoor == null)
        {
            Debug.LogError("❌ No main door assigned for this minigame.");
            return;
        }

        // Assign this door's ID to MinigameState
        MinigameState.CurrentDoorID = mainDoor.DoorID;

        // Save return position
        MinigameState.ReturnPosition = player.transform.position;
        Debug.Log($"📌 Saved return position: {MinigameState.ReturnPosition}");

        FindObjectOfType<SaveController>()?.SaveGame();

        string chosenScene = GetRandomScene();
        if (string.IsNullOrEmpty(chosenScene))
        {
            Debug.LogError("❌ No minigame scene chosen.");
            return;
        }

        SceneManager.LoadScene(chosenScene);
    }

    private string GetRandomScene()
    {
        if (minigameScenes == null || minigameScenes.Count == 0) return null;
        return minigameScenes[Random.Range(0, minigameScenes.Count)];
    }

    public void OnMinigameCompleted(bool won)
    {
        if (mainDoor == null) return;

        if (won)
        {
            OpenDoorByID(mainDoor.DoorID);

            if (!MinigameState.CompletedDoors.Contains(mainDoor.DoorID))
                MinigameState.CompletedDoors.Add(mainDoor.DoorID);
        }

        FindObjectOfType<SaveController2>()?.SaveGame();
        SceneManager.LoadScene("Level 1");
    }

    /// <summary>
    /// Opens all connected doors with the matching DoorID.
    /// </summary>
    private void OpenDoorByID(string doorID)
    {
        foreach (Door door in connectedDoors)
        {
            if (door != null && door.DoorID == doorID)
                door.OpenDoor();
        }
    }
}
