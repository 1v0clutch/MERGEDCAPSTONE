using UnityEngine;

public class DoorIDDebugger : MonoBehaviour
{
    void Start()
    {
        Door[] doors = FindObjectsOfType<Door>();
        foreach (var door in doors)
        {
            Debug.Log($"Door '{door.name}' has ID: {door.DoorID}");
        }
    }
}