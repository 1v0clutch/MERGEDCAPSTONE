
// Add at the top of both files
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections; // Add this line with other using directives

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text questionText;
    public TMP_InputField answerInput;
    public Button submitButton;
    public TMP_Text warningText;

    [Header("Mode Buttons")]
    public Button questionButton1;
    public Button questionButton2;
    public Button questionButton3;
    public Button debugButton;
    public Button timerButton;

    private Coroutine warningCoroutine;

    public void SetQuestionText(string text)
    {
        questionText.text = text;
    }

    public void SetAnswerInput(string text, bool interactable)
    {
        answerInput.text = text;
        answerInput.interactable = interactable;
    }

    public void SetSubmitButtonInteractable(bool interactable)
    {
        submitButton.interactable = interactable;
    }

    public void ShowWarningTemporarily(string message, float duration)
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
        }

        warningText.text = message;
        warningCoroutine = StartCoroutine(ClearWarningAfterDelay(duration));
    }

    private IEnumerator ClearWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        warningText.text = "";
        warningCoroutine = null;
    }

    public void SetAllModeButtonsInteractable(bool state)
    {
        questionButton1.interactable = state;
        questionButton2.interactable = state;
        questionButton3.interactable = state;
        debugButton.interactable = state;
        timerButton.interactable = state;
    }

    public void ClearErrorMessages()
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        warningText.text = "";
    }
}