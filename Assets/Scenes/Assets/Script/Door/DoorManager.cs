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
    [SerializeField] private GameObject interactButton;

    private string currentSignDoorID;
    private List<string> currentSignScenes;
    private GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // ✅ listen for scene loads
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainLevelSceneName)
        {
            // Clear null door references
            allDoors.RemoveAll(d => d.doorObject == null);

            // Re-register all doors in the scene
            foreach (var door in FindObjectsOfType<Door>())
            {
                RegisterDoor(door);
            }

            // Apply open/close state
            InitializeDoorsFromState();
        }
    }

    public void RegisterDoor(Door door)
    {
        if (!allDoors.Exists(d => d.doorID == door.DoorID))
            allDoors.Add(new DoorData { doorID = door.DoorID, doorObject = door });
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
        MinigameState.CurrentDoorID = doorID;
        MinigameState.ReturnPosition = player.transform.position;

        var doorData = allDoors.Find(d => d.doorID == doorID);
        if (doorData != null && !string.IsNullOrEmpty(doorData.minigameScene))
            SceneManager.LoadScene(doorData.minigameScene);
        else if (scenes != null && scenes.Count > 0)
            SceneManager.LoadScene(scenes[0]);
        else
            Debug.LogError($"No minigame scene assigned for door {doorID}");
    }

    public void FinishMinigame(bool won)
    {
        if (won && !string.IsNullOrEmpty(MinigameState.CurrentDoorID))
        {
            var doorID = MinigameState.CurrentDoorID;

            // ✅ Only open & award points if the door hasn't been completed before
            var isNewDoor = !MinigameState.CompletedDoors.Contains(doorID);

            var doorData = allDoors.Find(d => d.doorID == doorID);
            if (doorData != null && doorData.doorObject != null) // ✅ check if destroyed
            {
                doorData.doorObject.OpenDoor(isNewDoor);
            }
            else
            {
                Debug.LogWarning($"Door '{doorID}' was not found or has been destroyed — skipping OpenDoor()");
            }
            if (isNewDoor)
            {
                MinigameState.CompletedDoors.Add(doorID);
            }
        }

        FindObjectOfType<SaveController>()?.SaveGame();
        SceneManager.LoadScene(mainLevelSceneName);
    }

    public void InitializeDoorsFromState()
    {
        foreach (var data in allDoors)
        {
            if (MinigameState.CompletedDoors.Contains(data.doorID))
                data.doorObject.OpenDoor(false); // No points
            else
                data.doorObject.CloseDoor();
        }
    }

}
