using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignButtonActivator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactButton; 
    [SerializeField] private DoorMinigameManager minigameManager;

    private GameObject player;

    private void Start()
    {
        if (interactButton != null)
            interactButton.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        player = collision.gameObject;
        minigameManager?.SetPlayer(player);
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

    public void OnInteractButtonPressed()
    {
        if (player == null) return;

        if (MinigameState.MinigameCompleted && MinigameState.DoorShouldBeOpen)
        {
            Debug.Log("✅ Minigame already completed. Opening door.");
            minigameManager?.OnMinigameCompleted(true);
            return;
        }

        minigameManager?.StartMinigame();
    }
}

