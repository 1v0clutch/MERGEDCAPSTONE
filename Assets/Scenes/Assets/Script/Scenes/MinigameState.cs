using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MinigameState
{
    public static bool MinigameCompleted = false;
    public static bool DoorShouldBeOpen = false;

    public static Vector3 ReturnPosition = Vector3.zero;
    public static bool HasReturnedFromMinigame => MinigameCompleted;
}

