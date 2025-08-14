using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorMinigameManager : MonoBehaviour
{
    [Header("Minigame Settings")]
    [Tooltip("List of possible minigame scenes for this door.")]
    [SerializeField] private List<string> minigameScenes;

    [Tooltip("The door this minigame is linked to.")]
    [SerializeField] private Door connectedDoor;

    private GameObject player;

    private void Awake()
    {
        if (connectedDoor == null)
        {
            Debug.LogError($"❌ {name} has no connected Door assigned!");
            return;
        }
    }

    private void Start()
    {
        if (connectedDoor == null) return;

        string doorID = connectedDoor.DoorID;

        // ✅ Only open this door if *its* ID is in the completed list
        if (MinigameState.CompletedDoors.Contains(doorID))
        {
            connectedDoor.OpenDoor();
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
        if (connectedDoor == null)
        {
            Debug.LogError("❌ No connected door assigned to this manager.");
            return;
        }

        // Save this door's ID so we know which one to open
        MinigameState.CurrentDoorID = connectedDoor.DoorID;

        // Save return position
        MinigameState.ReturnPosition = player.transform.position;
        Debug.Log($"📌 Saved return position: {MinigameState.ReturnPosition}");

        FindObjectOfType<SaveController>()?.SaveGame();

        // Pick a random scene from this door's list
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
        if (connectedDoor == null) return;

        if (won)
        {
            connectedDoor.OpenDoor();

            if (!MinigameState.CompletedDoors.Contains(connectedDoor.DoorID))
                MinigameState.CompletedDoors.Add(connectedDoor.DoorID);
        }

        FindObjectOfType<SaveController2>()?.SaveGame();
        SceneManager.LoadScene("Level 1");
    }
}
