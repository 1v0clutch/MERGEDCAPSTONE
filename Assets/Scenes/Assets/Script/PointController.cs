// ===== UPDATED POINTCONTROLLER.CS =====
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PointController : MonoBehaviour
{
    public static PointController Instance;

    public int TotalPoints { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI pointsText;

    [Header("Point Values")]
    public int pointsPerEnemyKill = 100;
    public int pointsPerQuestionEnemyKill = 150;
    public int pointsPerDamageTaken = -25;
    public int pointsPerItemCollected = 50;
    public int pointsPerDoorOpened = 200;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("✅ PointController.Instance created");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log($"🎯 PointController.Start - Current points: {TotalPoints}");
        Debug.Log($"🎯 Pending points to apply: {MinigameState.PendingPoints}");
        
        // ✅ Apply pending points if any
        if (MinigameState.PendingPoints > 0)
        {
            Debug.Log($"🏆 PointController.Start applying {MinigameState.PendingPoints} pending points");
            AddPoints(MinigameState.PendingPoints);
            MinigameState.PendingPoints = 0;
            MinigameState.PendingRewardDoorID = null;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"🎯 PointController.OnSceneLoaded - Scene: {scene.name}");
        RebindUI();
        UpdateUI();
        
        // ✅ Another failsafe for pending points
        if (MinigameState.PendingPoints > 0)
        {
            Debug.Log($"🎯 OnSceneLoaded applying {MinigameState.PendingPoints} pending points");
            AddPoints(MinigameState.PendingPoints);
            MinigameState.PendingPoints = 0;
            MinigameState.PendingRewardDoorID = null;
        }
    }

    public void AddPoints(int amount)
    {
        int oldPoints = TotalPoints;
        TotalPoints += amount;
        UpdateUI();
        Debug.Log($"💰 Points updated: {oldPoints} + {amount} = {TotalPoints}");
    }

    public void SubtractPoints(int amount)
    {
        TotalPoints -= amount;
        UpdateUI();
    }

    public void SetTotalPoints(int value)
    {
        TotalPoints = value;
        UpdateUI();
    }

    public void NewGame()
    {
        SetTotalPoints(0);
    }

    public void EnemyKilled(bool isQuestionEnemy = false)
    {
        AddPoints(isQuestionEnemy ? pointsPerQuestionEnemyKill : pointsPerEnemyKill);
    }

    public void PlayerDamaged()
    {
        SubtractPoints(Mathf.Abs(pointsPerDamageTaken));
    }

    public void ItemCollected()
    {
        AddPoints(pointsPerItemCollected);
    }

    public void DoorOpened()
    {
        Debug.Log("🚪 DoorOpened method called (legacy)");
        AddPoints(pointsPerDoorOpened);
    }

    private void UpdateUI()
    {
        if (pointsText == null)
        {
            RebindUI();
        }
        if (pointsText != null)
        {
            pointsText.text = $"Points: {TotalPoints}";
            Debug.Log($"🟢 UI Updated: {pointsText.text}");
        }
        else
        {
            Debug.LogError("❌ No pointsText found for UI update!");
        }
    }

    public void RebindUI()
    {
        if (pointsText != null) 
        {
            Debug.Log("✅ PointsText already assigned in Inspector.");
            return;
        }

        var found = GameObject.Find("PointsText");
        if (found != null)
        {
            pointsText = found.GetComponent<TextMeshProUGUI>();
            pointsText.text = $"Points: {TotalPoints}";
            return;
        }


        Debug.LogWarning("⚠️ Could not find any PointsText in this scene.");
    }

    // ✅ Manual method for testing
    [ContextMenu("Add 200 Points")]
    public void TestAddPoints()
    {
        AddPoints(200);
    }
}