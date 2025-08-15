using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition; //
    public List<InventorySaveData> inventorySaveData;//
    public List<int> collectedItemIDs;//
    public List<int> completedLessons = new();//
    public List<int> unlockedLessonIDs;//
    public int lastLessonID = -1;//
    public List<Vector3> enemyPositions = new();//
    public List<Vector3> questionEnemyPositions = new();//
    public List<string> completedDoorIDs;//
    public int totalPoints;
    public bool minigameCompleted;
    public bool doorShouldBeOpen;
    public string lastMinigameDoorID; //
    public Vector3 returnPosition; //

    public int gemCount;
    public int playerHealth;
}
