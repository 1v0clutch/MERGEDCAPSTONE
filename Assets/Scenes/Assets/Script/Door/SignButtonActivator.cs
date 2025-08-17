using UnityEngine;
using System.Collections.Generic;

public class SignButtonActivator : MonoBehaviour
{
    [SerializeField] private string signID;
    [SerializeField] private string linkedDoorID;
    [SerializeField] private List<string> minigameScenes;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Collider2D myCollider = GetComponent<Collider2D>();
        if (player != null && myCollider.bounds.Contains(player.transform.position))
        {
            if (!MinigameState.CompletedDoors.Contains(linkedDoorID))
            {
                DoorManager.Instance.SetActiveSign(linkedDoorID, minigameScenes, player);
                DoorManager.Instance.ShowInteractButton(true);
                Debug.Log($"🔄 Player started inside sign {signID}, re-activated interact button.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ Don't show interact button if door is already completed
            if (MinigameState.CompletedDoors.Contains(linkedDoorID))
            {
                Debug.Log($"🚫 Door {linkedDoorID} already completed - sign inactive");
                return;
            }

            DoorManager.Instance.SetActiveSign(linkedDoorID, minigameScenes, player);
            DoorManager.Instance.ShowInteractButton(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DoorManager.Instance.ClearActiveSign();
            DoorManager.Instance.ShowInteractButton(false);
        }
    }
}