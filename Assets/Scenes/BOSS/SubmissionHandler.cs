public class SubmissionHandler
{
    private JDoodleAPI jdoodleAPI;
    private GameController gameController;
    private UIManager uiManager;

    public SubmissionHandler(JDoodleAPI api, GameController controller, UIManager ui)
    {
        jdoodleAPI = api;
        gameController = controller;
        uiManager = ui;
    }

    public void HandleCodeSubmission(string userInput, Question currentQ, bool isDebugMode, System.Action onSuccess, System.Action onFail)
    {
        if (Helper.HasSemanticErrors(userInput))
        {
            uiManager.ShowWarningTemporarily("Syntax error detected! Please check your code.", 2f);
            uiManager.SetAnswerInput(userInput, true);
            uiManager.SetSubmitButtonInteractable(true);
            return;
        }

        jdoodleAPI.RunCodeWithExpectedOutput(userInput, currentQ.expectedOutput, (isCodeCorrect, apiSuccess) =>
        {
            if (!apiSuccess)
            {
                uiManager.SetQuestionText("Submission failed! Try again.");
                uiManager.SetSubmitButtonInteractable(true);
                uiManager.SetAnswerInput(userInput, true);
                return;
            }

            if (isDebugMode)
            {
                if (Helper.CleanInput(userInput) == Helper.CleanInput(currentQ.answer))
                {
                    gameController.PlayerAttack();
                    onSuccess();
                }
                else
                {
                    gameController.OpponentAttack();
                    uiManager.ShowWarningTemporarily("Debug failed! Check your fix.", 2f);
                    uiManager.SetAnswerInput(userInput, true);
                    uiManager.SetSubmitButtonInteractable(true);
                    onFail();
                }
                return;
            }

            if (isCodeCorrect)
            {
                gameController.PlayerAttack();
                uiManager.SetAnswerInput("", false);
                onSuccess();
            }
            else
            {
                gameController.OpponentAttack();
                uiManager.ShowWarningTemporarily("Wrong output! Review your code.", 2f);
                uiManager.SetAnswerInput(userInput, true);
                uiManager.SetSubmitButtonInteractable(true);
                onFail();
            }
        }, currentQ.requiredKeyword);
    }
}