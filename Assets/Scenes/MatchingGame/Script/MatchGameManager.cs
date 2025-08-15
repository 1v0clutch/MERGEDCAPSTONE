using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchGameManager : MonoBehaviour
{
    [SerializeField] private int maxAttempts = 3;
    [SerializeField] private GameObject gamePanelToDisable; // Optional UI/parent object to close/hide
    private int attemptsLeft;

    private List<MatchConnection> connections = new List<MatchConnection>();

    private void Start()
    {
        attemptsLeft = maxAttempts;

        foreach (ObjectMatchingGame obj in FindObjectsOfType<ObjectMatchingGame>())
            obj.Unlock();
        foreach (ObjectMatchform obj in FindObjectsOfType<ObjectMatchform>())
            obj.Unlock();
    }

    public void AddConnection(ObjectMatchingGame source, ObjectMatchform target, LineRenderer line)
    {
        // 🛑 Do not allow connections to locked cards
        if (source.IsLocked || target.IsLocked)
        {
            Debug.Log("⛔ This card is locked and cannot be reconnected.");
            Destroy(line.gameObject);
            return;
        }

        // Remove any existing connections from this source or target
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            if (connections[i].source == source || connections[i].target == target)
            {
                if (connections[i].line != null)
                    Destroy(connections[i].line.gameObject);

                connections.RemoveAt(i);
            }
        }

        connections.Add(new MatchConnection(source, target, line));
    }


    public void CheckConnections()
    {
        if (attemptsLeft <= 0)
        {
            Debug.Log("❌ No attempts left. Game Over!");
            ExitToLevel(false); // force fail
            return;
        }

        List<MatchConnection> correctConnections = new List<MatchConnection>();

        foreach (var connection in connections)
        {
            if (connection.IsCorrect())
            {
                connection.line.startColor = Color.green;
                connection.line.endColor = Color.green;
                connection.line.widthMultiplier = 0.1f;

                connection.source.Lock();
                connection.target.Lock();
                connection.IsLocked = true;

                correctConnections.Add(connection);
            }
            else
            {
                connection.line.startColor = Color.red;
                connection.line.endColor = Color.red;
                Destroy(connection.line.gameObject, 0.5f);
            }
        }

        connections = correctConnections;
        attemptsLeft--;
        Debug.Log($"🔁 Attempts left: {attemptsLeft}");

        bool won = AllCardsMatched();

        if (won)
        {
            Debug.Log("🎉 All matches correct!");
            ExitToLevel(true); // success
        }
        else if (attemptsLeft <= 0)
        {
            Debug.Log("❌ Attempts exhausted, returning to level.");
            ExitToLevel(false); // fail
        }
        else
        {
            Debug.Log("🔄 Not all matches correct, but attempts remain. Try again.");
        }
    }

    private bool AllCardsMatched()
    {
        int totalPairs = FindObjectsOfType<ObjectMatchingGame>().Length;
        
        // If we don't have enough correct matches, return false
        if (connections.Count < totalPairs)
            return false;

        return connections.TrueForAll(c => c.IsLocked && c.IsCorrect());
    }

    public void ClearAllConnections()
    {
        foreach (var connection in connections)
        {
            if (connection.line != null)
                Destroy(connection.line.gameObject);
        }
        connections.Clear();
    }
    private void ExitToLevel(bool won)
    {
        DoorManager.Instance.FinishMinigame(won);
        FindObjectOfType<SaveController2>()?.SaveGame();
        SceneManager.LoadScene("Level 1");
        DoorManager.Instance.InitializeDoorsFromState();
    }
}