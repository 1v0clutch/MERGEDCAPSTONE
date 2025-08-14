using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
   public Vector3 playerPosition;
   public List<InventorySaveData> inventorySaveData;
   public List<int> collectedItemIDs;

   public List<int> completedLessons = new(); // ✅ store lesson IDs
   public int lastLessonID = -1; // ✅ store which lesson was last open
   public List<int> unlockedLessonIDs = new(); // ✅ new
   public List<Vector3> enemyPositions = new();
   public List<Vector3> questionEnemyPositions = new();
   public int gemCount;
   public int playerHealth;
   public bool doorOpened;
   public bool minigameCompleted;
   public bool doorShouldBeOpen;
   public Vector3 returnPosition;

   public List<string> completedDoorIDs = new List<string>();
}
