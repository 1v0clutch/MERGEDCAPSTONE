using UnityEngine;
using TMPro;

public class PointController : MonoBehaviour
{
    public static PointController Instance;

    public int TotalPoints { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI pointsText; // Assign in Inspector

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
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int amount)
    {
        TotalPoints += amount;
        UpdateUI();
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
        AddPoints(pointsPerDoorOpened);
    }

    private void UpdateUI()
    {
        if (pointsText != null)
            pointsText.text = $"Points: {TotalPoints}";
    }
}
