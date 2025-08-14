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

        // Update only minigame-related fields
        saveData.minigameCompleted = MinigameState.MinigameCompleted;
        saveData.doorShouldBeOpen = MinigameState.DoorShouldBeOpen;
        saveData.returnPosition = MinigameState.ReturnPosition;
        saveData.completedDoorIDs = new List<string>(MinigameState.CompletedDoors);

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
        Debug.Log("✅ Minigame save merged: " + JsonUtility.ToJson(saveData));
    }
}
