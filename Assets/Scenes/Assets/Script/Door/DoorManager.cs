// ===== UPDATED DOORMANAGER.CS =====
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
            SceneManager.sceneLoaded += OnSceneLoaded;
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
            Debug.Log($"🏠 DoorManager.OnSceneLoaded - Main level loaded");
            Debug.Log($"🔍 Pending points: {MinigameState.PendingPoints}, Reward door: {MinigameState.PendingRewardDoorID}");
            Debug.Log($"🔍 PointController.Instance exists: {PointController.Instance != null}");

            // Re-register doors (handles reloaded scene case)
            allDoors.RemoveAll(d => d.doorObject == null);
            foreach (var door in FindObjectsOfType<Door>())
                RegisterDoor(door);

            // ✅ Try to award pending points immediately
            TryAwardPendingPoints();

            // Initialize door visual states
            InitializeDoorsFromState();

            // ✅ Start coroutine as backup in case PointController wasn't ready
            if (MinigameState.PendingPoints > 0)
            {
                StartCoroutine(DelayedPointAward());
            }
        }

        MinigameState.LastCompletedDoorID = null;
    }

    private void TryAwardPendingPoints()
    {
        if (MinigameState.PendingPoints > 0 && !string.IsNullOrEmpty(MinigameState.PendingRewardDoorID))
        {
            Debug.Log($"🎯 Attempting to award {MinigameState.PendingPoints} points for door {MinigameState.PendingRewardDoorID}");
            
            if (PointController.Instance != null)
            {
                PointController.Instance.AddPoints(MinigameState.PendingPoints);
                Debug.Log($"🏆 SUCCESS! Awarded {MinigameState.PendingPoints} points for door {MinigameState.PendingRewardDoorID}");
                
                // Clear pending state
                MinigameState.PendingPoints = 0;
                MinigameState.PendingRewardDoorID = null;
                return;
            }
            else
            {
                Debug.LogWarning("⚠️ PointController.Instance is null in TryAwardPendingPoints");
            }
        }
        else
        {
            Debug.Log("ℹ️ No pending points to award");
        }
    }

    private System.Collections.IEnumerator DelayedPointAward()
    {
        float timeout = 5f;
        float elapsed = 0f;

        while (MinigameState.PendingPoints > 0 && elapsed < timeout)
        {
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;

            if (PointController.Instance != null)
            {
                Debug.Log($"🕐 DelayedPointAward: Awarding {MinigameState.PendingPoints} points after {elapsed}s delay");
                PointController.Instance.AddPoints(MinigameState.PendingPoints);
                
                MinigameState.PendingPoints = 0;
                MinigameState.PendingRewardDoorID = null;
                yield break;
            }
        }

        if (MinigameState.PendingPoints > 0)
        {
            Debug.LogError($"❌ FAILED to award {MinigameState.PendingPoints} points after {timeout}s timeout!");
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
        StartMinigameForDoor(currentSignDoorID, currentSignScenes);
    }

    public void StartMinigameForDoor(string doorID, List<string> scenes)
    {
        MinigameState.CurrentDoorID = doorID;
        Debug.Log($"🎮 Starting minigame for door: {doorID}");

        var doorData = allDoors.Find(d => d.doorID == doorID);
        string sceneToLoad = doorData?.minigameScene ?? (scenes != null && scenes.Count > 0 ? scenes[0] : null);

        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
        else
            Debug.LogError($"❌ No minigame scene assigned for door {doorID}");
    }

    public void InitializeDoorsFromState()
    {
        Debug.Log($"🔄 Initializing {allDoors.Count} doors from state. Completed doors: [{string.Join(", ", MinigameState.CompletedDoors)}]");

        foreach (var data in allDoors)
        {
            if (data.doorObject == null) continue;

            if (MinigameState.CompletedDoors.Contains(data.doorID))
            {
                data.doorObject.OpenDoor();
                Debug.Log($"🔓 Opened door {data.doorID} (visual only)");
            }
            else
            {
                data.doorObject.CloseDoor();
                Debug.Log($"🔒 Closed door {data.doorID}");
            }
        }
    }

    // ✅ Manual method to force point award (for testing)
    [ContextMenu("Force Award Pending Points")]
    public void ForceAwardPendingPoints()
    {
        Debug.Log($"🔧 Force awarding pending points: {MinigameState.PendingPoints}");
        TryAwardPendingPoints();
    }
}