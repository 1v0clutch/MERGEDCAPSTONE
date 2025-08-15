using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
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

    [Header("Door Data")]
    public List<DoorData> allDoors = new List<DoorData>();
    public string mainLevelSceneName = "Level 1";

    [Header("UI References")]
    [SerializeField] private GameObject interactButton; // Assign the single UI button here

    // State tracking
    private string currentSignDoorID;
    private List<string> currentSignScenes;
    private GameObject player;

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
        // Not storing sign refs here for now, just for compatibility
    }

    public void SetActiveSign(string doorID, List<string> scenes, GameObject player)
    {
        currentSignDoorID = doorID;
        currentSignScenes = scenes;
        this.player = player;
    }

    public void ClearActiveSign()
    {
        currentSignDoorID = null;
        currentSignScenes = null;
        player = null;
    }

    public void ShowInteractButton(bool show)
    {
        if (interactButton != null)
            interactButton.SetActive(show);
    }

    public void OnInteractButtonPressed()
    {
        if (string.IsNullOrEmpty(currentSignDoorID))
        {
            Debug.LogWarning("⚠ No active sign to interact with.");
            return;
        }
        MinigameState.ReturnPosition = player.transform.position;
        FindObjectOfType<SaveController>()?.SaveGame();
        StartMinigameForDoor(currentSignDoorID, player, currentSignScenes);
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
            bool isNewDoor = !MinigameState.CompletedDoors.Contains(MinigameState.CurrentDoorID);

            if (isNewDoor)
            {
                MinigameState.CompletedDoors.Add(MinigameState.CurrentDoorID); // ✅ mark as completed
                PointController.Instance?.DoorOpened(); // ✅ award points
            }
        }

        FindObjectOfType<SaveController>()?.SaveGame();
        SceneManager.LoadScene(mainLevelSceneName);
    }


    public void InitializeDoorsFromState()
    {
        // ✅ Open all doors that have been completed
        foreach (var data in allDoors)
        {
            if (MinigameState.CompletedDoors.Contains(data.doorID))
            {
                // Open without giving points when loading from save
                data.doorObject.OpenDoor(false);
            }
            else
            {
                data.doorObject.CloseDoor();
            }
        }
    }

}
