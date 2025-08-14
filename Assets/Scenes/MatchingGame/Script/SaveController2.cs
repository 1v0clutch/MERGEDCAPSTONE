using UnityEngine;
using System.IO;

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
            // Load existing full save
            saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
        }
        else
        {
            saveData = new SaveData();
        }

        // ✅ Only update minigame-related fields
        saveData.minigameCompleted = MinigameState.MinigameCompleted;
        saveData.doorShouldBeOpen = MinigameState.DoorShouldBeOpen;
        saveData.returnPosition = MinigameState.ReturnPosition;

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
        Debug.Log("✅ Minigame save merged with existing data: " + JsonUtility.ToJson(saveData));
    }
}

