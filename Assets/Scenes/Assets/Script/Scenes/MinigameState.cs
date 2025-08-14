using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MinigameState
{
    // New per-door system
    public static Vector3 ReturnPosition;
    public static string CurrentDoorID;
    public static HashSet<string> CompletedDoors = new HashSet<string>();

    // Backward compatibility for old scripts
    public static bool MinigameCompleted
    {
        get
        {
            // Completed if the current door ID is in the set
            return !string.IsNullOrEmpty(CurrentDoorID) && CompletedDoors.Contains(CurrentDoorID);
        }
        set
        {
            if (!string.IsNullOrEmpty(CurrentDoorID))
            {
                if (value)
                    CompletedDoors.Add(CurrentDoorID);
                else
                    CompletedDoors.Remove(CurrentDoorID);
            }
        }
    }

    public static bool DoorShouldBeOpen
    {
        get
        {
            // Same as MinigameCompleted for old system
            return MinigameCompleted;
        }
        set
        {
            // For compatibility, treat setting DoorShouldBeOpen to true as marking it completed
            MinigameCompleted = value;
        }
    }
}
