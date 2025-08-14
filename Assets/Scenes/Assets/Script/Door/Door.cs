using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private string doorID;
    public string DoorID => doorID;

    private void Awake()
    {
        if (string.IsNullOrEmpty(doorID))
            doorID = System.Guid.NewGuid().ToString();

        DoorManager.Instance?.RegisterDoor(this);
    }

    public void OpenDoor()
    {
        // Your animation/disable collider logic here
        Debug.Log($"✅ Door '{doorID}' opened.");
    }

    public void CloseDoor()
    {
        // Your animation/enable collider logic here
        Debug.Log($"🔒 Door '{doorID}' closed.");
    }
}
