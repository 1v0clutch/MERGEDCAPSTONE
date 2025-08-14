
// Add at the top of both files
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintSystem : MonoBehaviour
{
    [Header("Hint Settings")]
    public Button hintButton;
    public TMP_Text hintCounterText;

    private int hintCount = 3;
    private List<string> currentDebugHints = new List<string>();

    public void SetupHints(Question question)
    {
        hintCount = 3;
        hintButton.interactable = true;
        hintButton.gameObject.SetActive(true);
        hintCounterText.text = "Hints left: 3";

        currentDebugHints.Clear();

        if (question.prompt.Contains("Missing semicolon"))
        {
            currentDebugHints.Add("Check for line endings. Is something missing?");
            currentDebugHints.Add("Each statement should end with a semicolon.");
            currentDebugHints.Add("Line 1 may be missing something.");
        }
        else if (question.prompt.Contains("For loop syntax"))
        {
            currentDebugHints.Add("Loops need proper semicolons in the header.");
            currentDebugHints.Add("Compare with the usual for(int i=0; i<5; i++) format.");
            currentDebugHints.Add("One semicolon seems missing between 'i = 0' and 'i < 5'.");
        }
        else
        {
            currentDebugHints.Add("Watch out for off-by-one errors.");
            currentDebugHints.Add("Pay attention to array indices.");
            currentDebugHints.Add("Does the array access go out of bounds?");
        }
    }

    public string GetNextHint()
    {
        if (hintCount <= 0) return string.Empty;

        hintCount--;
        hintCounterText.text = $"Hints: {hintCount}";

        if (hintCount <= 0)
        {
            hintButton.interactable = false;
        }

        return hintCount <= currentDebugHints.Count ?
            currentDebugHints[3 - hintCount - 1] :
            "Keep trying! Look at the structure of the code.";
    }

    public void HideHints()
    {
        hintButton.gameObject.SetActive(false);
    }
}