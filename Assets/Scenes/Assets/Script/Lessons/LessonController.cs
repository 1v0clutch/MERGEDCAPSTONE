using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class LessonController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject infoPanel;
    public GameObject activityPanel;
    public GameObject lessonPanel;

    [Header("UI References")]
    public TMP_Text infoText;
    public TMP_Text syntaxText;
    public TMP_Text questionText;
    public TMP_InputField inputField;
    public TMP_Text terminalOutput;

    [Header("Buttons")]
    public GameObject restartLessonButton;
    public GameObject nextToLessonButton;
    public Button submitButton;

    // 🧠 New: Using Item.cs
    private Item currentItem;

    private int currentQuestionIndex;
    private bool[] answeredCorrectly;
    private bool hasCompletedActivity = false;

    public void StartLessonByID(Item item, bool isInitial = false)
    {
        currentItem = item;
        currentQuestionIndex = 0;
        hasCompletedActivity = false;

        if (currentItem.activityQuestions == null || currentItem.activityQuestions.Count == 0)
        {
            Debug.LogWarning("⚠️ No activity questions found in Item.");
            return;
        }

        answeredCorrectly = new bool[currentItem.activityQuestions.Count];

        ShowInfoPanel();

        restartLessonButton?.SetActive(!isInitial);
        nextToLessonButton?.SetActive(false);
    }

    public void ShowInfoPanel()
    {
        infoPanel.SetActive(true);
        activityPanel.SetActive(false);
        lessonPanel.SetActive(false); 

        infoText.text = currentItem.description;
        syntaxText.text = currentItem.syntaxExplanation;
    }

    public void ShowActivityPanel()
    {
        infoPanel.SetActive(false);
        activityPanel.SetActive(true);
        lessonPanel.SetActive(false);
        DisplayCurrentQuestion();
    }

    private void DisplayCurrentQuestion()
    {
        if (currentQuestionIndex >= currentItem.activityQuestions.Count)
        {
            hasCompletedActivity = true;
            terminalOutput.text += "\n✅ All questions complete!";
            nextToLessonButton?.SetActive(true);
            return;
        }

        var question = currentItem.activityQuestions[currentQuestionIndex];
        questionText.text = question.question;
        terminalOutput.text = "";

        if (answeredCorrectly[currentQuestionIndex])
        {
            inputField.text = question.acceptedAnswers[0];
            inputField.interactable = false;
            submitButton.interactable = false;
        }
        else
        {
            inputField.text = "";
            inputField.interactable = true;
            submitButton.interactable = true;
        }
    }

    public void SubmitAnswer()
    {
        if (currentItem == null || currentQuestionIndex >= currentItem.activityQuestions.Count)
        {
            Debug.LogWarning("❌ No valid lesson or question to answer.");
            return;
        }

        string userAnswer = inputField.text.Trim().ToLower();
        var question = currentItem.activityQuestions[currentQuestionIndex];

        bool isCorrect = question.acceptedAnswers.Any(ans => userAnswer == ans.ToLower());

        if (isCorrect)
        {
            terminalOutput.text = question.pseudoOutputIfCorrect;
            answeredCorrectly[currentQuestionIndex] = true;

            inputField.interactable = false;
            submitButton.interactable = false;

            currentQuestionIndex++;

            if (currentQuestionIndex >= currentItem.activityQuestions.Count)
            {
                hasCompletedActivity = true;
                terminalOutput.text += "\n✅ All questions complete!";
                nextToLessonButton?.SetActive(true);
            }
            else
            {
                Invoke(nameof(DisplayCurrentQuestion), 1f);
            }
        }
        else
        {
            terminalOutput.text = "❌ Incorrect, try again.";
        }
    }

    public void OnNextToLessonClicked()
    {
        if (hasCompletedActivity && currentItem != null)
        {
            // Close Activity and Info Panels
            activityPanel.SetActive(false);
            infoPanel.SetActive(false);

            // ✅ Open Lesson Board from LessonBoardManager
            LessonBoardManager.Instance.ShowLesson(currentItem.ID, currentItem.description, allowClose: false);
        }
    }

    public void OnResetLessonClicked()
    {
        for (int i = 0; i < answeredCorrectly.Length; i++)
            answeredCorrectly[i] = false;

        currentQuestionIndex = 0;
        hasCompletedActivity = false;
        ShowInfoPanel();
    }

    public void ResetLesson()
    {
        StartLessonByID(currentItem, isInitial: false);
    }

    public void GoBackToInfoPanel()
    {
        ShowInfoPanel();
    }

    public void GoBackToActivityPanel()
    {
        ShowActivityPanel();
    }
}
