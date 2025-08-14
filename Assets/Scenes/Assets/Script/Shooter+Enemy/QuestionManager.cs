using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string answer;
    }

    public List<Question> questions;
    private Question currentQuestion;

    public GameObject questionPanel;
    public TMP_Text questionText;
    public TMP_InputField answerInput;
    public Button submitButton;
    public TMP_Text timerText;
    public TMP_Text feedbackText; // ✅ Add this in Inspector

    private float timeRemaining = 10f;
    private bool isQuestionActive = false;
    private bool answeredCorrectly = false;
    private System.Action<bool> onQuestionFinished;

    private void Start()
    {
        questionPanel.SetActive(false);
        submitButton.onClick.AddListener(CheckAnswer);
    }

    void Update()
    {
        if (!isQuestionActive) return;

        timeRemaining -= Time.unscaledDeltaTime;
        timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";

        if (!answeredCorrectly && timeRemaining <= 0f)
        {
            feedbackText.text = "⏰ Time's up!";
            StartCoroutine(EndQuestionAfterDelay(false, 1.5f));
        }
    }

    public void TriggerQuestion(System.Action<bool> onFinishCallback, float customTime = 10f)
    {
        if (questions.Count == 0)
        {
            Debug.LogError("❌ No questions available!");
            onFinishCallback?.Invoke(false);
            return;
        }

        currentQuestion = questions.OrderBy(x => Random.value).First();
        onQuestionFinished = onFinishCallback;

        answerInput.text = "";
        questionText.text = currentQuestion.questionText;
        timeRemaining = customTime;
        isQuestionActive = true;
        answeredCorrectly = false;

        questionPanel.SetActive(true);
        feedbackText.text = ""; // Clear feedback
        Time.timeScale = 0f;
    }

    void CheckAnswer()
    {
        if (!isQuestionActive || answeredCorrectly) return;

        bool isCorrect = answerInput.text.Trim().ToLower() == currentQuestion.answer.ToLower();

        if (isCorrect)
        {
            answeredCorrectly = true;
            feedbackText.text = "✅ Correct!";
            StartCoroutine(EndQuestionAfterDelay(true, 1.5f)); // Close after 1.5s
        }
        else
        {
            feedbackText.text = "❌ Incorrect! Try again or wait.";
            // Panel stays open, timer continues
        }
    }

    IEnumerator EndQuestionAfterDelay(bool success, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        EndQuestion(success);
    }

    void EndQuestion(bool success)
    {
        isQuestionActive = false;
        questionPanel.SetActive(false);
        Time.timeScale = 1f;
        onQuestionFinished?.Invoke(success);
    }
}
