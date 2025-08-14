using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Player Settings")]
    public Slider playerHealthBar;
    public GameObject player;
    private int playerHealth = 100;

    [Header("Opponent Settings")]
    public Slider opponentHealthBar;
    public GameObject opponent;
    private int opponentHealth = 100;

    [Header("UI References")]
    public GameObject quizPanel;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;

    [Header("Dependencies")]
    public QuizManager quizManager;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        playerHealth = 100;
        opponentHealth = 100;
        playerHealthBar.value = playerHealth;
        opponentHealthBar.value = opponentHealth;

        player.GetComponent<SpriteRenderer>().color = Color.white;
        opponent.GetComponent<SpriteRenderer>().color = Color.white;

        gameOverPanel.SetActive(false);
        quizPanel.SetActive(true);
    }

    public void PlayerAttack()
    {
        opponentHealth = Mathf.Max(0, opponentHealth - 20);
        opponentHealthBar.value = opponentHealth;
        CheckWeakness(opponent, opponentHealth);
        CheckGameOver();
    }

    public void OpponentAttack()
    {
        playerHealth = Mathf.Max(0, playerHealth - 20);
        playerHealthBar.value = playerHealth;
        CheckWeakness(player, playerHealth);
        CheckGameOver();
    }

    private void CheckWeakness(GameObject character, int health)
    {
        if (health <= 30)
        {
            character.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }


    private void CheckGameOver()
    {
        if (playerHealth <= 0 || opponentHealth <= 0)
        {
            bool playerWon = opponentHealth <= 0;
            HandleGameOver(playerWon);
        }
    }

    private void HandleGameOver(bool playerWon)
    {
        gameOverText.text = playerWon ? "You Win!" : "You Lose!";
        gameOverPanel.SetActive(true);
        quizPanel.SetActive(false);

        if (quizManager != null)
        {
            quizManager.EndGameEarly();
        }
        else
        {
            Debug.LogWarning("QuizManager reference missing in GameController");
        }
    }

    public void RestartGame()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Add this single line:
        if (quizManager != null) quizManager.ResetTimerRounds();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // For debugging purposes only
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerAttack();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            OpponentAttack();
        }
    }
}