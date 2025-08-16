// ===== UPDATED MATCHGAMEMANAGER.CS =====
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchGameManager : MonoBehaviour
{
    [SerializeField] private int maxAttempts = 3;
    [SerializeField] private GameObject gamePanelToDisable;
    private int attemptsLeft;
    private List<MatchConnection> connections = new List<MatchConnection>();

    private void Start()
    {
        attemptsLeft = maxAttempts;
        Debug.Log($"🎮 MatchGameManager started. Current door ID: {MinigameState.CurrentDoorID}");

        foreach (ObjectMatchingGame obj in FindObjectsOfType<ObjectMatchingGame>())
            obj.Unlock();
        foreach (ObjectMatchform obj in FindObjectsOfType<ObjectMatchform>())
            obj.Unlock();
    }

    public void AddConnection(ObjectMatchingGame source, ObjectMatchform target, LineRenderer line)
    {
        if (source.IsLocked || target.IsLocked)
        {
            Debug.Log("⛔ This card is locked and cannot be reconnected.");
            Destroy(line.gameObject);
            return;
        }

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
            ExitToLevel(false);
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
            ExitToLevel(true);
        }
        else if (attemptsLeft <= 0)
        {
            Debug.Log("❌ Attempts exhausted, returning to level.");
            ExitToLevel(false);
        }
        else
        {
            Debug.Log("🔄 Not all matches correct, but attempts remain. Try again.");
        }
    }

    private bool AllCardsMatched()
    {
        int totalPairs = FindObjectsOfType<ObjectMatchingGame>().Length;
        
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
        Debug.Log($"🎯 MatchGameManager exiting with won: {won}, door: {MinigameState.CurrentDoorID}");
        
        // ✅ Mark completion in static state
        if (won && !string.IsNullOrEmpty(MinigameState.CurrentDoorID))
        {
            bool isNewCompletion = !MinigameState.CompletedDoors.Contains(MinigameState.CurrentDoorID);
            Debug.Log($"🔍 Is new completion: {isNewCompletion}");
            
            if (isNewCompletion)
            {
                MinigameState.CompletedDoors.Add(MinigameState.CurrentDoorID);
                MinigameState.PendingPoints = 200; // Set your door points value
                MinigameState.PendingRewardDoorID = MinigameState.CurrentDoorID;
                
                Debug.Log($"🎉 NEW COMPLETION! Set pending: {MinigameState.PendingPoints} points for door {MinigameState.CurrentDoorID}");
            }
            else
            {
                Debug.Log($"🔄 Door {MinigameState.CurrentDoorID} already completed - no points set");
                MinigameState.PendingPoints = 0; // Make sure no points for repeat
            }
            
            MinigameState.MinigameCompleted = true;
            MinigameState.DoorShouldBeOpen = true;
        }

        var saveController = FindObjectOfType<SaveController2>();
        if (saveController != null)
        {
            saveController.SaveGame();
            Debug.Log("💾 Game saved via SaveController2");
        }
        else
        {
            Debug.LogWarning("⚠️ No SaveController2 found");
        }
        
        Debug.Log($"🏠 Loading Level 1. Pending points: {MinigameState.PendingPoints}");
        SceneManager.LoadScene("Level 1");
    }
}