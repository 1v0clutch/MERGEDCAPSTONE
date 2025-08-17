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

    public void OpenDoor(bool fromSave = false)
    {

        gameObject.SetActive(false); // purely visual
        Debug.Log($"🚪 Door {doorID} opened. FromSave? {fromSave}");

        if (!fromSave && PointController.Instance != null)
        {
            PointController.Instance.DoorOpened();
            Debug.Log($"🎉 Awarded points for door {doorID}");
        }

    }



    public void CloseDoor()
    {
        if (this != null && gameObject != null)
        {
            gameObject.SetActive(true);
            Debug.Log($"🔒 Door '{doorID}' closed.");
        }
    }
}
