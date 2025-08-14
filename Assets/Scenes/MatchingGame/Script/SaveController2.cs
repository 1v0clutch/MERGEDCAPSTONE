using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveController2 : MonoBehaviour
{
    private string saveLocation;

    private void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public void SaveGame()
    {
        SaveData saveData;

        if (File.Exists(saveLocation))
        {
            saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
        }
        else
        {
            saveData = new SaveData();
        }

        // ✅ Always save full per-door state
        saveData.completedDoorIDs = new List<string>(MinigameState.CompletedDoors);
        saveData.minigameCompleted = MinigameState.MinigameCompleted;
        saveData.doorShouldBeOpen = MinigameState.DoorShouldBeOpen;
        saveData.returnPosition = MinigameState.ReturnPosition;
        saveData.lastMinigameDoorID = MinigameState.CurrentDoorID; // new field you add to SaveData

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
        Debug.Log("✅ Minigame save merged with existing data: " + JsonUtility.ToJson(saveData));

        // Reset after save so it doesn't interfere with the next door
        MinigameState.MinigameCompleted = false;
        MinigameState.CurrentDoorID = null;
    }
}
