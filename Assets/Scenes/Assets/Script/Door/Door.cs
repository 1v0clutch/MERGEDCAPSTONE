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
        gameObject.SetActive(false);
        
        PointController.Instance?.DoorOpened();

        Debug.Log($"✅ Door '{doorID}' opened. Points awarded");
    }

    public void CloseDoor()
    {
        gameObject.SetActive(true);
        Debug.Log($"🔒 Door '{doorID}' closed.");
    }
}
