using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [TextArea(3, 10)]
    public string tutorialMessage; // 👈 Editable message in Inspector
    public float typingSpeed = 0.01f;
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public Button okButton;

    private bool hasTriggered = false;

    private void Start()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (okButton != null)
        {
            okButton.onClick.AddListener(CloseTutorial);
            okButton.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;
        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(ShowTutorial());
        }
    }

    IEnumerator ShowTutorial()
    {
        Time.timeScale = 0f;
        tutorialPanel.SetActive(true);
        tutorialText.text = "";
        okButton.gameObject.SetActive(false);

        // Typewriter effect
        foreach (char c in tutorialMessage)
        {
            tutorialText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        yield return new WaitForSecondsRealtime(0.5f); // Short delay before showing OK
        okButton.gameObject.SetActive(true);
    }

    void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
