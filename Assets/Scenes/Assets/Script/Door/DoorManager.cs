using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance;

    [System.Serializable]
    public class DoorData
    {
        public string doorID;
        public Door doorObject;
        public string minigameScene;
    }

    public List<DoorData> allDoors = new List<DoorData>();
    public string mainLevelSceneName = "Level 1";

    private Dictionary<string, SignButtonActivator> allSigns = new Dictionary<string, SignButtonActivator>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterDoor(Door door)
    {
        if (!allDoors.Exists(d => d.doorID == door.DoorID))
        {
            allDoors.Add(new DoorData { doorID = door.DoorID, doorObject = door });
        }
    }

    public void RegisterSign(string signID, SignButtonActivator sign)
    {
        if (!allSigns.ContainsKey(signID))
        {
            allSigns.Add(signID, sign);
        }
    }

    public void InitializeDoorsFromState()
    {
        foreach (var data in allDoors)
        {
            if (MinigameState.CompletedDoors.Contains(data.doorID))
                data.doorObject.OpenDoor();
            else
                data.doorObject.CloseDoor();
        }
    }

    public void StartMinigameForDoor(string doorID, GameObject player, List<string> scenes)
    {
        if (string.IsNullOrEmpty(doorID))
        {
            Debug.LogError("❌ Door ID is null or empty.");
            return;
        }

        MinigameState.CurrentDoorID = doorID;
        MinigameState.ReturnPosition = player.transform.position;

        var doorData = allDoors.Find(d => d.doorID == doorID);
        if (doorData != null && !string.IsNullOrEmpty(doorData.minigameScene))
        {
            SceneManager.LoadScene(doorData.minigameScene);
        }
        else if (scenes != null && scenes.Count > 0)
        {
            SceneManager.LoadScene(scenes[0]);
        }
        else
        {
            Debug.LogError($"No minigame scene assigned for door {doorID}");
        }
    }

    public void FinishMinigame(bool won)
    {
        if (won && !string.IsNullOrEmpty(MinigameState.CurrentDoorID))
        {
            MinigameState.CompletedDoors.Add(MinigameState.CurrentDoorID);
        }

        FindObjectOfType<SaveController>()?.SaveGame();
        SceneManager.LoadScene(mainLevelSceneName);
    }
}
