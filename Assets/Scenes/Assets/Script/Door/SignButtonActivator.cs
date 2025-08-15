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
        //DoorManager.Instance?.RegisterSign(signID, this);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ❌ Don't activate if door already completed
            if (MinigameState.CompletedDoors.Contains(linkedDoorID))
                return;

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
