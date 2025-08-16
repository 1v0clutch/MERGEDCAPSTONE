// ===== UPDATED GAMECONTROL.CS =====
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    [SerializeField] private Transform[] pictures;
    [SerializeField] private GameObject winText;

    public static bool youWin;

    private void Start()
    {
        winText.SetActive(false);
        youWin = false;
        Debug.Log($"🎮 GameControl started. Current door ID: {MinigameState.CurrentDoorID}");
    }

    private void Update()
    {
        if (!youWin && AllPicturesAligned())
        {
            youWin = true;
            winText.SetActive(true);
            Debug.Log("🎉 Puzzle completed!");
            HandlePuzzleCompleted();
        }
    }

    private bool AllPicturesAligned()
    {
        for (int i = 0; i < pictures.Length; i++)
        {
            if (!Mathf.Approximately(pictures[i].rotation.eulerAngles.z % 360f, 0f))
                return false;
        }
        return true;
    }

    private void HandlePuzzleCompleted()
    {
        Debug.Log($"🎯 Handling puzzle completion for door: {MinigameState.CurrentDoorID}");
        
        // ✅ Mark completion in static state
        if (!string.IsNullOrEmpty(MinigameState.CurrentDoorID))
        {
            bool isNewCompletion = !MinigameState.CompletedDoors.Contains(MinigameState.CurrentDoorID);
            Debug.Log($"🔍 Is new completion: {isNewCompletion}");
            
            if (isNewCompletion)
            {
                MinigameState.CompletedDoors.Add(MinigameState.CurrentDoorID);
                MinigameState.PendingPoints = 200; // Set your door points value
                MinigameState.PendingRewardDoorID = MinigameState.CurrentDoorID;
                
                Debug.Log($"🎉 NEW COMPLETION! Set pending: {MinigameState.PendingPoints} points for door {MinigameState.CurrentDoorID}");
            }
            else
            {
                Debug.Log($"🔄 Door {MinigameState.CurrentDoorID} already completed - no points set");
                MinigameState.PendingPoints = 0; // Make sure no points are set for repeat
            }
            
            MinigameState.MinigameCompleted = true;
            MinigameState.DoorShouldBeOpen = true;
        }
        else
        {
            Debug.LogError("❌ No CurrentDoorID set!");
        }

        // Save and return
        var saveController = FindObjectOfType<SaveController3>();
        if (saveController != null)
        {
            saveController.SaveGame();
            Debug.Log("💾 Game saved via SaveController3");
        }
        else
        {
            Debug.LogWarning("⚠️ No SaveController3 found");
        }
        
        Debug.Log($"🏠 Loading Level 1. Pending points: {MinigameState.PendingPoints}");
        SceneManager.LoadScene("Level 1");
    }
}

