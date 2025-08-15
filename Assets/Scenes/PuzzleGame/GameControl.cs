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
    }

    private void Update()
    {
        if (!youWin && AllPicturesAligned())
        {
            youWin = true;
            winText.SetActive(true);

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
        // Mark the minigame state
        MinigameState.MinigameCompleted = true;
        MinigameState.DoorShouldBeOpen = true;
        MinigameState.CompletedDoors.Add(MinigameState.CurrentDoorID);

        // Save using SaveController2 in this scene
        var saveController = FindObjectOfType<SaveController2>();
        if (saveController != null)
        {
            saveController.SaveGame();
        }

        // Return to main level
        ExitToLevel(true);
    }

    private void ExitToLevel(bool won)
    {
        DoorManager.Instance.FinishMinigame(won);
        FindObjectOfType<SaveController3>()?.SaveGame();
        SceneManager.LoadScene("Level 1");
    }



}
