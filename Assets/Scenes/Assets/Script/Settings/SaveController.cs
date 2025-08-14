using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    public GameObject enemyPrefab; // assign in Inspector
    private GemCounter gemCounter;
    public GameObject questionEnemyPrefab;
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
            // Don't save immediately — delay until player is properly initialized
            inventoryController.ClearInventorySlots();
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

        // Minigame return logic
        if (MinigameState.MinigameCompleted)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = MinigameState.ReturnPosition;
                Debug.Log("📍 Returned to position after Minigame: " + MinigameState.ReturnPosition);
            }
        }

        // Open door if passed
        if (MinigameState.MinigameCompleted && MinigameState.DoorShouldBeOpen)
        {
            Door door = FindObjectOfType<Door>();
            if (door != null)
            {
                door.OpenDoor();
                Debug.Log("✅ Door opened from minigame result.");
            }
        }

        // ✅ Mark game as initialized
        GameState.IsGameInitialized = true;

        // ✅ Now it's safe to save the initial game state
        if (!File.Exists(saveLocation))
        {
            SaveGame(); // <-- move this to AFTER init is fully complete
        }
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
            enemyPositions = new List<Vector3>()
        };
        saveData.completedDoorIDs = new List<string>(MinigameState.CompletedDoors);
        saveData.minigameCompleted = MinigameState.MinigameCompleted;
        saveData.doorShouldBeOpen = MinigameState.DoorShouldBeOpen;
        saveData.returnPosition = MinigameState.ReturnPosition;
        saveData.gemCount = gemCounter.GetGemCount();
        saveData.enemyPositions = new List<Vector3>();
        saveData.questionEnemyPositions = new List<Vector3>();

        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            saveData.enemyPositions.Add(enemy.transform.position);
        }

        foreach (QuestionEnemy qEnemy in FindObjectsOfType<QuestionEnemy>())
        {
            saveData.questionEnemyPositions.Add(qEnemy.transform.position);
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            saveData.playerPosition = player.transform.position;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            PlayerMovement pm = playerObj != null ? playerObj.GetComponent<PlayerMovement>() : null;

            if (pm != null)
            {
                saveData.playerHealth = Mathf.Max(pm.currentHealth, 1); // never save 0
            }
        }
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
    }


    public void LoadGame()
    {
        if (!File.Exists(saveLocation)) return;

        SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

        // Destroy existing enemies
        foreach (Enemy e in FindObjectsOfType<Enemy>())
            Destroy(e.gameObject);
        foreach (QuestionEnemy qe in FindObjectsOfType<QuestionEnemy>())
            Destroy(qe.gameObject);

        // Re-instantiate enemies
        List<GameObject> restoredEnemies = new List<GameObject>();
        foreach (Vector3 enemyPos in saveData.enemyPositions)
        {
            if (enemyPrefab != null)
                restoredEnemies.Add(Instantiate(enemyPrefab, enemyPos, Quaternion.identity));
        }
        foreach (Vector3 qEnemyPos in saveData.questionEnemyPositions)
        {
            if (questionEnemyPrefab != null)
                restoredEnemies.Add(Instantiate(questionEnemyPrefab, qEnemyPos, Quaternion.identity));
        }

        // 3️⃣ Register restored enemies to nearby spawners
        foreach (Spawner spawner in FindObjectsOfType<Spawner>())
        {
            foreach (GameObject enemy in restoredEnemies)
            {
                if (Vector2.Distance(spawner.transform.position, enemy.transform.position) < 15f)
                {
                    spawner.RegisterExistingEnemy(enemy);
                }
            }

            // Disable only if it's already full
            if (spawner.AliveEnemyCount >= spawner.maxEnemiesAlive)
            {
                spawner.enabled = false;
                Debug.Log($"🛑 Spawner '{spawner.name}' disabled — already full ({spawner.AliveEnemyCount}/{spawner.maxEnemiesAlive})");
            }
        }

        // Assign player data
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();

            if (pm != null)
            {
                pm.currentHealth = (saveData.playerHealth <= 0) ? pm.maxHealth : saveData.playerHealth;

                if (pm.healthSlider != null)
                    pm.healthSlider.value = pm.currentHealth;
            }

        }

        // Restore other systems
        inventoryController.SetInventoryItems(saveData.inventorySaveData);
        LessonBoardManager.Instance.RegisterUnlockedLessons(inventoryController.GetUnlockedLessonIDs());
        LessonBoardManager.Instance.completedLessons = new HashSet<int>(saveData.completedLessons);
        LessonBoardManager.Instance.lastOpenedLessonID = saveData.lastLessonID;

        Item[] worldItems = FindObjectsOfType<Item>();
        foreach (Item item in worldItems)
        {
            if (saveData.collectedItemIDs.Contains(item.ID))
                Destroy(item.gameObject);
        }

        gemCounter.SetGemCount(saveData.gemCount);
        // ✅ Restore minigame state
        MinigameState.CompletedDoors = new HashSet<string>(saveData.completedDoorIDs);
        MinigameState.MinigameCompleted = saveData.minigameCompleted;
        MinigameState.DoorShouldBeOpen = saveData.doorShouldBeOpen;
        MinigameState.ReturnPosition = saveData.returnPosition;

        // ✅ Set player position (use return position if coming from minigame)
        if (player != null)
        {
            if (MinigameState.MinigameCompleted)
            {
                player.transform.position = MinigameState.ReturnPosition;
                Debug.Log("📍 Returned to position after Minigame: " + MinigameState.ReturnPosition);
            }
            else
            {
                player.transform.position = saveData.playerPosition;
            }
        }

        // ✅ Door state based only on MinigameState
        Door door = FindObjectOfType<Door>();
        if (door != null)
        {
            if (MinigameState.DoorShouldBeOpen)
            {
                door.OpenDoor();
                Debug.Log("✅ Door opened from Minigame result");
            }
            else
            {
                door.CloseDoor();
                Debug.Log("🔒 Door is locked");
            }
        }
    }
}
