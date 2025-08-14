using System.Collections.Generic;
using UnityEngine;

public static class MinigameState
{
    public static bool MinigameCompleted = false;
    public static bool DoorShouldBeOpen = false;

    public static string CurrentDoorID = null;
    public static Vector3 ReturnPosition;

    public static HashSet<string> CompletedDoors = new HashSet<string>();
}
