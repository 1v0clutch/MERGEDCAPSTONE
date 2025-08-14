using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveController3 : MonoBehaviour
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

        // ✅ Save full per-door completion state
        saveData.completedDoorIDs = new List<string>(MinigameState.CompletedDoors);
        saveData.minigameCompleted = MinigameState.MinigameCompleted;
        saveData.doorShouldBeOpen = MinigameState.DoorShouldBeOpen;
        saveData.returnPosition = MinigameState.ReturnPosition;
        saveData.lastMinigameDoorID = MinigameState.CurrentDoorID; // new field

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
        Debug.Log("✅ Minigame save merged: " + JsonUtility.ToJson(saveData));

        // ✅ Prevent global state from messing with next door
        MinigameState.MinigameCompleted = false;
        MinigameState.CurrentDoorID = null;
    }
}
