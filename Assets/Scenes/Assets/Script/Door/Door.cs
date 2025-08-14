using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Unique ID for this door (used for saving/loading state). Leave blank to auto-generate.")]
    [SerializeField] private string doorID;

    public string DoorID => doorID;

    private static HashSet<string> usedIDs = new HashSet<string>();

    private void Awake()
    {
        if (string.IsNullOrEmpty(doorID) || usedIDs.Contains(doorID))
        {
            doorID = Guid.NewGuid().ToString();
            Debug.Log($"🆔 Generated new unique DoorID for '{gameObject.name}': {doorID}");
        }
        usedIDs.Add(doorID);
    }

    public void AssignAsCurrentDoor()
    {
        MinigameState.CurrentDoorID = doorID;
        Debug.Log($"🚪 Current door set to: {doorID}");
    }

    public void OpenDoor()
    {
        gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        gameObject.SetActive(true);
    }
}