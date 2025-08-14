// Updated QuizManager.cs

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    [Header("Dependencies")]
    public GameController gameController;
    public JDoodleAPI jdoodleAPI;

    [Header("Managers")]
    public QuestionData questionData;
    public TimerManager timerManager;
    public HintSystem hintSystem;
    public UIManager uiManager;

    private List<Question> selectedQuestions = new List<Question>();
    private QuestionSelector questionSelector;
    private QuestionTracker questionTracker;
    private SubmissionHandler submissionHandler;

    private Question currentTimerQuestion;
    private int timerRoundsCompleted = 0;
    private const int MAX_TIMER_ROUNDS = 3;

    private bool isAnyModeActive = false;
    private bool isDebugMode = false;
    private bool challengeStartedDuringDelay = false;

    private int _timerRoundsCompleted = 0;
    public int TimerRoundsCompleted => _timerRoundsCompleted;

    private void Start()
    {
        questionData.Initialize();
        questionSelector = new QuestionSelector(questionData);
        questionTracker = new QuestionTracker();
        submissionHandler = new SubmissionHandler(jdoodleAPI, gameController, uiManager);

        StartBattle();

        uiManager.submitButton.onClick.AddListener(OnSubmit);
        uiManager.questionButton1.onClick.AddListener(() => OnQuestionSelected(0));
        uiManager.questionButton2.onClick.AddListener(() => OnQuestionSelected(1));
        uiManager.questionButton3.onClick.AddListener(() => OnQuestionSelected(2));
        uiManager.debugButton.onClick.AddListener(() => OnQuestionSelected(3));
        uiManager.timerButton.onClick.AddListener(StartTimerMode);

        hintSystem.hintButton.onClick.AddListener(OnHintClicked);
        hintSystem.HideHints();
    }

    private void Update()
    {
        if (timerManager.IsTimerModeActive)
        {
            timerManager.UpdateTimer();
            if (timerManager.HasTimeExpired()) OnTimerExpired();
        }
    }

    private void StartBattle()
    {
        selectedQuestions = questionSelector.GetSelectedQuestions();
        questionTracker.Initialize(selectedQuestions.Count);

        UnlockButtons();
        uiManager.SetQuestionText("Welcome to the Code Battle! Tap a box below to begin your challenge.");
        uiManager.SetAnswerInput("", false);
        uiManager.SetSubmitButtonInteractable(false);
    }

    private void StartTimerMode()
    {
        if (_timerRoundsCompleted >= MAX_TIMER_ROUNDS) return;
        _timerRoundsCompleted++;

        if (timerRoundsCompleted >= MAX_TIMER_ROUNDS) return;

        challengeStartedDuringDelay = true;
        if (isAnyModeActive || questionData.timerQuestions.Count == 0) return;

        LockButtons();
        uiManager.ClearErrorMessages();
        isDebugMode = false;
        timerManager.StartTimerMode();

        currentTimerQuestion = questionData.timerQuestions[Random.Range(0, questionData.timerQuestions.Count)];
        timerRoundsCompleted++;

        uiManager.SetQuestionText($"Timer Round {timerRoundsCompleted}/3: " + currentTimerQuestion.prompt);
        uiManager.SetAnswerInput(currentTimerQuestion.buggyCode, true);
        uiManager.SetSubmitButtonInteractable(true);
        hintSystem.HideHints();
    }

    private void OnQuestionSelected(int index)
    {
        challengeStartedDuringDelay = true;
        if (isAnyModeActive || questionTracker.totalAnswered >= selectedQuestions.Count) return;

        LockButtons();
        uiManager.ClearErrorMessages();

        bool found = false;
        for (int i = index; i < selectedQuestions.Count; i += 4)
        {
            if (i >= selectedQuestions.Count) break;

            if (!questionTracker.IsAnswered(i))
            {
                questionTracker.currentIndex = i;
                var q = selectedQuestions[i];
                uiManager.SetQuestionText(q.prompt);

                if (q.isCodeQuestion && !string.IsNullOrEmpty(q.buggyCode))
                {
                    isDebugMode = true;
                    uiManager.SetAnswerInput(q.buggyCode, true);
                    hintSystem.SetupHints(q);
                }
                else
                {
                    isDebugMode = false;
                    uiManager.SetAnswerInput("", true);
                    hintSystem.HideHints();
                }

                uiManager.SetSubmitButtonInteractable(true);
                found = true;
                break;
            }
        }

        if (!found)
        {
            uiManager.SetQuestionText("All challenges in this path conquered! Choose a new sequence to continue the battle.");
            uiManager.SetAnswerInput("", false);
            uiManager.SetSubmitButtonInteractable(false);
            UnlockButtons();
        }
    }

    private void OnSubmit()
    {
        if (timerManager.IsTimerModeActive)
        {
            HandleTimerSubmission();
            return;
        }

        int index = questionTracker.currentIndex;
        if (index < 0 || questionTracker.IsAnswered(index)) return;

        string userInput = Helper.CleanInput(uiManager.answerInput.text);
        if (string.IsNullOrWhiteSpace(userInput))
        {
            uiManager.ShowWarningTemporarily("Please enter an answer!", 1f);
            return;
        }

        Question currentQ = selectedQuestions[index];

        if (!currentQ.isCodeQuestion)
        {
            bool isCorrect = userInput.ToLower() == currentQ.answer.ToLower();

            if (isCorrect)
            {
                gameController.PlayerAttack();
            }
            else
            {
                gameController.OpponentAttack();
                uiManager.ShowWarningTemporarily("Wrong answer!", 2f);
            }

            FinalizeQuestion(); //Progress continues regardless
        }
        else
        {
            //NEW: Handle code-based questions here
            submissionHandler.HandleCodeSubmission(userInput, currentQ, isDebugMode,
                onSuccess: () => FinalizeQuestion(),
                onFail: () => { /* Let the user try again */ });
        }
    }

    private void HandleTimerSubmission()
    {
        if (!timerManager.IsTimerModeActive) return;

        string userInput = Helper.CleanInput(uiManager.answerInput.text);
        if (string.IsNullOrWhiteSpace(userInput))
        {
            uiManager.ShowWarningTemporarily("Please fix the code!", 1f);
            return;
        }

        bool isCorrect = Helper.CleanInput(userInput) == Helper.CleanInput(currentTimerQuestion.answer);
        if (isCorrect)
        {
            gameController.PlayerAttack();
            uiManager.SetQuestionText($"Round {timerRoundsCompleted}/3 Complete!");
            EndTimerMode(); //End only when correct
        }
        else
        {
            gameController.OpponentAttack();
            uiManager.ShowWarningTemporarily("Incorrect! Try again...", 1.5f);
            // Keep timer running for retries
        }
    }


    private void OnTimerExpired()
    {
        gameController.OpponentAttack();
        uiManager.SetQuestionText("TIME'S UP! The bugs escaped!");
        EndTimerMode();
    }

    private void EndTimerMode()
    {
        if (_timerRoundsCompleted >= 3) uiManager.timerButton.interactable = false;
        if (timerRoundsCompleted >= MAX_TIMER_ROUNDS) uiManager.SetQuestionText("Timer Challenge Complete!");

        challengeStartedDuringDelay = false;
        timerManager.StopTimerMode();
        uiManager.SetAnswerInput("", false);
        uiManager.SetSubmitButtonInteractable(false);
        UnlockButtons();
        StartCoroutine(ReturnToMainMode());
        hintSystem.HideHints();
    }

    private IEnumerator ReturnToMainMode()
    {
        yield return new WaitForSeconds(2f);
        if (!challengeStartedDuringDelay)
        {
            uiManager.SetQuestionText("Choose your next challenge!");
        }
    }

    private void OnHintClicked()
    {
        string hint = hintSystem.GetNextHint();
        if (!string.IsNullOrEmpty(hint))
        {
            uiManager.ShowWarningTemporarily($"Hint: {hint}", 3f);
            uiManager.SetSubmitButtonInteractable(false);
            StartCoroutine(ReenableRunButton(3f));
        }
    }

    private IEnumerator ReenableRunButton(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isDebugMode && questionTracker.currentIndex >= 0 && !questionTracker.IsAnswered(questionTracker.currentIndex))
        {
            uiManager.SetSubmitButtonInteractable(true);
        }
    }

    private void FinalizeQuestion()
    {
        int index = questionTracker.currentIndex;
        if (index < 0 || index >= selectedQuestions.Count)
        {
            UnlockButtons();
            return;
        }

        questionTracker.MarkAnswered(index);
        uiManager.ClearErrorMessages();
        uiManager.SetAnswerInput("", false);
        uiManager.SetSubmitButtonInteractable(false);

        if (questionTracker.totalAnswered >= selectedQuestions.Count)
        {
            uiManager.SetQuestionText("Game Over! Well played, warrior of code!");
            EndGameEarly();
        }
        else
        {
            uiManager.SetQuestionText("Choose your next challenge!");
            UnlockButtons();
        }

        hintSystem.HideHints();
    }

    private void LockButtons() => SetButtonsInteractable(false);
    private void UnlockButtons() => SetButtonsInteractable(true);
    private void SetButtonsInteractable(bool value)
    {
        isAnyModeActive = !value;
        uiManager.SetAllModeButtonsInteractable(value);
    }

    public void ResetTimerRounds()
    {
        _timerRoundsCompleted = 0;
        uiManager.timerButton.interactable = true;
    }

    public void EndGameEarly()
    {
        isAnyModeActive = false;
        uiManager.SetAnswerInput("", false);
        uiManager.SetSubmitButtonInteractable(false);
        uiManager.SetAllModeButtonsInteractable(false);
    }
}
