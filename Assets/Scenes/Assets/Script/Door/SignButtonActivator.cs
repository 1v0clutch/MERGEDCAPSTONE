using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignButtonActivator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactButton; // UI Button GameObject
    [SerializeField] private Door connectedDoor;
    [SerializeField] private string minigameSceneName = "MATCHGAME";

    private GameObject player;

    private void Start()
    {
        if (interactButton != null)
            interactButton.SetActive(false); // Make sure it's hidden at start
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        player = collision.gameObject;

        if (interactButton != null)
            interactButton.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        player = null;

        if (interactButton != null)
            interactButton.SetActive(false);
    }

    // ✅ Call this from the UI Button’s OnClick() event
    public void OnInteractButtonPressed()
    {
        if (player == null) return;

        if (MinigameState.MinigameCompleted && MinigameState.DoorShouldBeOpen)
        {
            Debug.Log("✅ Minigame already completed. Opening door.");
            connectedDoor.OpenDoor();
            return;
        }

        // Save player's current position
        MinigameState.ReturnPosition = player.transform.position;
        Debug.Log("📌 Saved return position: " + MinigameState.ReturnPosition);

        // Save game before scene switch
        FindObjectOfType<SaveController>()?.SaveGame();

        Debug.Log("🎮 Loading Minigame...");
        SceneManager.LoadScene(minigameSceneName);
    }
}
