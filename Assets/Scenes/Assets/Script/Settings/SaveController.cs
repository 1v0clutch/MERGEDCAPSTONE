using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    private GemCounter gemCounter;

    public GameObject enemyPrefab;         // Assign in Inspector
    public GameObject questionEnemyPrefab; // Assign in Inspector

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        inventoryController = FindObjectOfType<InventoryController>();
        inventoryController.InitializeInventory();

        gemCounter = FindObjectOfType<GemCounter>();

        if (File.Exists(saveLocation))
        {
            LoadGame();
        }
        else
        {
            inventoryController.ClearInventorySlots();
            PointController.Instance.SetTotalPoints(0);
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerMovement pm = playerObj.GetComponent<PlayerMovement>();
            if (pm != null && pm.healthSlider != null)
            {
                pm.healthSlider.value = pm.currentHealth;
            }
        }

        // ✅ Apply "return from minigame" logic ONLY if last door was completed



        DoorManager.Instance?.InitializeDoorsFromState();

        if (!File.Exists(saveLocation))
        {
            SaveGame();
        }
        GameState.IsGameInitialized = true;
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            inventorySaveData = inventoryController.GetInventoryItems(),
            collectedItemIDs = inventoryController.GetCollectedItemIDs(),
            completedLessons = LessonBoardManager.Instance.completedLessons.ToList(),
            unlockedLessonIDs = inventoryController.GetUnlockedLessonIDs(),
            lastLessonID = LessonBoardManager.Instance.lastOpenedLessonID,
            enemyPositions = new List<Vector3>(),
            questionEnemyPositions = new List<Vector3>(),
            completedDoorIDs = new List<string>(MinigameState.CompletedDoors),
            gemCount = gemCounter.GetGemCount(),
            totalPoints = PointController.Instance != null ? PointController.Instance.TotalPoints : 0
        };

        // Save enemy positions
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            saveData.enemyPositions.Add(enemy.transform.position);

        foreach (QuestionEnemy qEnemy in FindObjectsOfType<QuestionEnemy>())
            saveData.questionEnemyPositions.Add(qEnemy.transform.position);

        // Save player health
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerMovement pm = playerObj.GetComponent<PlayerMovement>();
            if (pm != null)
                saveData.playerHealth = Mathf.Max(pm.currentHealth, 1);
        }

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
        Debug.Log($"💾 Game Saved. TotalPoints={saveData.totalPoints}");

    }

    public void LoadGame()
    {
        if (!File.Exists(saveLocation)) return;

        SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

        SyncCompletedDoors(saveData.completedDoorIDs);

        if (PointController.Instance != null)
        {
            // ✅ Take max so we never lose freshly awarded points
            int finalPoints = Mathf.Max(PointController.Instance.TotalPoints, saveData.totalPoints);
            PointController.Instance.SetTotalPoints(finalPoints);
            Debug.Log($"💾 Restored points = {finalPoints}");
        }

        
        // Destroy existing enemies
        foreach (Enemy e in FindObjectsOfType<Enemy>())
            Destroy(e.gameObject);
        foreach (QuestionEnemy qe in FindObjectsOfType<QuestionEnemy>())
            Destroy(qe.gameObject);

        // Respawn enemies from save
        List<GameObject> restoredEnemies = new List<GameObject>();
        foreach (Vector3 pos in saveData.enemyPositions)
            if (enemyPrefab != null)
                restoredEnemies.Add(Instantiate(enemyPrefab, pos, Quaternion.identity));

        foreach (Vector3 pos in saveData.questionEnemyPositions)
            if (questionEnemyPrefab != null)
                restoredEnemies.Add(Instantiate(questionEnemyPrefab, pos, Quaternion.identity));

        // Register restored enemies to spawners
        foreach (Spawner spawner in FindObjectsOfType<Spawner>())
        {
            foreach (GameObject enemy in restoredEnemies)
            {
                if (Vector2.Distance(spawner.transform.position, enemy.transform.position) < 15f)
                    spawner.RegisterExistingEnemy(enemy);
            }

            if (spawner.AliveEnemyCount >= spawner.maxEnemiesAlive)
            {
                spawner.enabled = false;
                Debug.Log($"🛑 Spawner '{spawner.name}' disabled — already full.");
            }
        }

        // Restore inventory & lessons
        inventoryController.SetInventoryItems(saveData.inventorySaveData);
        LessonBoardManager.Instance.RegisterUnlockedLessons(inventoryController.GetUnlockedLessonIDs());
        LessonBoardManager.Instance.completedLessons = new HashSet<int>(saveData.completedLessons);
        LessonBoardManager.Instance.lastOpenedLessonID = saveData.lastLessonID;

        // Remove collected world items
        foreach (Item item in FindObjectsOfType<Item>())
            if (saveData.collectedItemIDs.Contains(item.ID))
                Destroy(item.gameObject);

        // Restore gems
        gemCounter.SetGemCount(saveData.gemCount);

        // ✅ Restore minigame state
        MinigameState.CompletedDoors = new HashSet<string>(saveData.completedDoorIDs);


        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerMovement pm = playerObj.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.currentHealth = (saveData.playerHealth <= 0) ? pm.maxHealth : saveData.playerHealth;
                if (pm.healthSlider != null)
                    pm.healthSlider.value = pm.currentHealth;
            }

            // ✅ Position player correctly
            if (!string.IsNullOrEmpty(saveData.lastMinigameDoorID) &&
                saveData.completedDoorIDs.Contains(saveData.lastMinigameDoorID))
            {
                playerObj.transform.position = saveData.returnPosition;
                Debug.Log("📍 Returned to position after minigame.");
            }
            else
            {
                playerObj.transform.position = saveData.playerPosition;
            }
        }

    }
    private void SyncCompletedDoors(List<string> savedCompletedDoors)
    {
        if (MinigameState.CompletedDoors == null)
            MinigameState.CompletedDoors = new HashSet<string>();

        foreach (var doorID in savedCompletedDoors)
        {
            // If the door wasn't already completed, add it
            if (!MinigameState.CompletedDoors.Contains(doorID))
            {
                MinigameState.CompletedDoors.Add(doorID);

                // 🚫 DO NOT give points here, because this is a restore
                // Points are only awarded when the minigame is *first* won
            }
        }
    }

}
