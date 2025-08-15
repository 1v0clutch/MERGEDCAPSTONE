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

    public void OpenDoor(bool awardPoints = true)
    {
        // Prevent re-awarding points for doors already completed
        bool isNewDoor = !MinigameState.CompletedDoors.Contains(doorID);

        gameObject.SetActive(false);

        PointController.Instance?.DoorOpened();
     

        Debug.Log($"✅ Door '{doorID}' opened. Points awarded: {awardPoints && isNewDoor}");
    }


    public void CloseDoor()
    {
        gameObject.SetActive(true);
        Debug.Log($"🔒 Door '{doorID}' closed.");
    }
}
