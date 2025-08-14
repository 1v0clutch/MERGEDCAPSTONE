using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LessonBoardManager : MonoBehaviour
{
    [Header("Control")]
    public GameObject closeButton;
    [Header("UI Panels")]
    public GameObject lessonBoardPanel;

    [Header("Block Content Area")]
    public Transform blockArea;

    [Header("Drop Slot Area")]
    public Transform dropArea;

    [Header("Text + Output")]
    public TMP_Text instructionText;
    public TMP_Text outputTerminal;
    public TMP_Text itemDescriptionText;
    public TMP_Text challengeText;

    [Header("Prefabs")]
    public GameObject blockPrefab;
    public GameObject dropSlotPrefab; // new
    public bool IsLessonUnlocked(int id) => unlockedLessonIDs.Contains(id);
    private int currentLessonID;
    private HashSet<int> unlockedLessonIDs = new();

    private Dictionary<int, List<(string, string)>> itemLessons = new()
    {
        { 1, new List<(string, string)> {
            ("print('Hello')", "Print Hello"),
            ("print(i)", "Print i")
        }},
        { 2, new List<(string, string)> {
            ("x = 5", "Set x to 5"),
            ("if x > 3:", "If Statement"),
            ("print('Big')", "Print Big")
        }},
        { 3, new List<(string, string)> {
            ("x = 5", "Set x to 5"),
            ("if x > 3:", "If Statement"),
            ("print('Big')", "Print Big")
        }},
        { 4, new List<(string, string)> {
            ("1", "1"),
            ("5", "5"),
            ("6", "6")
        }},
        { 5, new List<(string, string)> {
            ("3", "3"),
            ("5", "5"),
            ("10", "10")
        }},
        { 6, new List<(string, string)> {
            ("array[0]", "array[0]"),
            ("array[1]", "array[1]"),
            ("array[2]", "array[2]"),
            ("array[3]", "array[3]"),
            ("array[4]", "array[4]")
        }}
    };

    private Dictionary<int, LessonValidationRule> lessonValidationRules = new()
    {
        { 1, new LessonValidationRule(new List<string> { "print('Hello')", "print(i)" }) },
        { 2, new LessonValidationRule(new List<string> { "x = 5", "if x > 3:", "print('Big')" }) },
        { 3, new LessonValidationRule(new List<string> { "x = 5", "if x > 3:", "print('Big')" }) },
        { 4, new LessonValidationRule(new List<string> { "Equal" }, useParsed: true) },
        { 5, new LessonValidationRule(new List<string> { "Hello", "Hello", "Hello" }, useParsed: true) },
    };


    public static LessonBoardManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterUnlockedLessons(List<int> ids)
    {
        unlockedLessonIDs = new HashSet<int>(ids);
        Debug.Log("✅ Unlocked lessons: " + string.Join(", ", ids));
    }
    public void ShowLesson(int itemID, string itemDescription, bool allowClose = false)
    {
        Time.timeScale = 0f;
        ClearLessonBoard(); // 🧽 Ensure reset each time

        if (closeButton != null)
            closeButton.SetActive(allowClose);

        lessonBoardPanel.SetActive(true);

        currentLessonID = itemID;
        lastOpenedLessonID = itemID;

        outputTerminal.text = "";
        instructionText.text = "🧠 Solve the challenge!";

        if (itemDescriptionText != null)
            itemDescriptionText.text = itemDescription;

        if (challengeText != null)
            challengeText.text = $"🧠 Drag code blocks to solve: {itemDescription.ToLower()}";

        if (itemLessons.TryGetValue(itemID, out var blocks))
        {
            // ✅ Limit drop slots to just one for itemID == 4 (conditional statement lesson)
            int dropSlotCount = itemID == 4 ? 1 : blocks.Count;

            for (int i = 0; i < dropSlotCount; i++)
            {
                if (dropSlotPrefab != null)
                {
                    GameObject slot = Instantiate(dropSlotPrefab, dropArea);

                    // Ensure it's marked as a terminal slot
                    DropSlot dropSlotComponent = slot.GetComponent<DropSlot>();
                    if (dropSlotComponent != null)
                        dropSlotComponent.isTerminalSlot = true;
                }
            }

            // 🔁 Always create all blocks regardless of itemID
            for (int i = 0; i < blocks.Count; i++)
            {
                GameObject blockGO = Instantiate(blockPrefab, blockArea);
                LessonBlock block = blockGO.GetComponent<LessonBlock>();

                if (block == null)
                {
                    Debug.LogError("LessonBlock missing on prefab.");
                    continue;
                }

                block.statementText = blocks[i].Item1;
                block.originalParent = blockArea;

                if (block.label != null)
                    block.label.text = blocks[i].Item2;
            }
        }
    }

    public void RunLesson()
    {
        outputTerminal.text = "";
        bool allValid = true;
        List<string> submitted = new();

        // 🔁 Dynamically get all DropSlot components inside dropArea
        DropSlot[] dropSlots = dropArea.GetComponentsInChildren<DropSlot>();

        foreach (DropSlot dropSlot in dropSlots)
        {
            // ✅ Skip non-terminal slots (e.g., from Content area)
            if (!dropSlot.isTerminalSlot)
                continue;

            dropSlot.ClearBlock(); // Clear old block
            LessonBlock block = dropSlot.GetComponentInChildren<LessonBlock>();

            if (block != null)
            {
                dropSlot.AssignBlock(block);
                string code = block.statementText.Trim();
                submitted.Add(code);
                outputTerminal.text += $"→ {ParseLine(code)}\n";

                if (!IsSyntaxValid(code))
                {
                    allValid = false;
                    HighlightSlot(dropSlot.transform, Color.red);
                }
                else
                {
                    HighlightSlot(dropSlot.transform, Color.green);
                }
            }
            else
            {
                allValid = false;
                HighlightSlot(dropSlot.transform, Color.yellow);
            }
        }

        // Validate against correct answer for current lesson
        // ✅ Only validate order and exact match if not dynamic (e.g., lesson 1, 2, 3)
        if (lessonValidationRules.TryGetValue(currentLessonID, out var rule))
        {
            List<string> checkList = rule.UseParsedOutput
                ? submitted.Select(ParseLine).ToList()
                : submitted;

            if (checkList.Count != rule.ExpectedValues.Count ||
                !checkList.SequenceEqual(rule.ExpectedValues))
            {
                allValid = false;
            }
        }


        instructionText.text = allValid
            ? "✅ Lesson complete!"
            : "❌ Check syntax and order.";

        if (allValid)
        {
            if (!completedLessons.Contains(currentLessonID))
            {
                StartCoroutine(CloseLessonBoardWithDelay());
            }

            MarkLessonComplete(currentLessonID);
        }
    }

    private void HighlightSlot(Transform slot, Color color)
    {
        Image slotImage = slot.GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.color = color;
        }
    }

    public void ClearLessonBoard()
    {
        // 🧹 Destroy all draggable blocks from the block area (left panel)
        foreach (Transform child in blockArea)
        {
            Destroy(child.gameObject);
        }

        // 🧹 Destroy all drop slots (right side), including any manually placed in the hierarchy
        foreach (Transform child in dropArea)
        {
            Destroy(child.gameObject);
        }

        // 🧽 Clear terminal output and reset UI instruction
        outputTerminal.text = "";
        instructionText.text = "🧠 Solve the challenge!";
    }


    private bool IsSyntaxValid(string line)
    {
        if (currentLessonID == 4) // Accept plain numbers for condition evaluation
        {
            return int.TryParse(line.Trim(), out _);
        }
        return line.StartsWith("print(") ||
               line.StartsWith("for ") ||
               line.StartsWith("if ") ||
               line.Contains("=");
    }

    private string ParseLine(string line)
    {
        line = line.Trim();

        switch (currentLessonID)
        {
            case 4: // Conditional
                if (int.TryParse(line, out int val))
                {
                    if (val > 5) return "Greater";
                    if (val < 5) return "Lesser";
                    return "Equal";
                }
                return "Invalid Input";

            case 5: // For Loop
                if (int.TryParse(line, out int loopVal))
                {
                    return string.Join("\n", Enumerable.Repeat("Hello", loopVal));
                }
                return "Invalid Number";

            case 6: // Array access
                if (line.StartsWith("array[") && line.EndsWith("]"))
                {
                    string indexStr = line.Substring(6, line.Length - 7);
                    if (int.TryParse(indexStr, out int index) && index >= 0 && index < 5)
                    {
                        int[] array = { 1, 2, 3, 4, 5 };
                        return array[index].ToString();
                    }
                    return "Index Out of Bounds";
                }
                return "Invalid Expression";

            default:
                // Default print logic
                if (line.Contains("print("))
                {
                    int start = line.IndexOf("(") + 1;
                    int end = line.LastIndexOf(")");
                    if (start < end)
                    {
                        return line.Substring(start, end - start).Trim('\'', '"');
                    }
                }
                return line;
        }
    }

    private IEnumerator CloseLessonBoardWithDelay()
    {
        yield return new WaitForSecondsRealtime(2f); // ✅ unaffected by timeScale
        lessonBoardPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public HashSet<int> completedLessons = new();
    public int lastOpenedLessonID = -1;

    public void MarkLessonComplete(int lessonID)
    {
        if (!completedLessons.Contains(lessonID))
            completedLessons.Add(lessonID);

        lastOpenedLessonID = lessonID;
    }
    public void LoadCustomLesson(int itemID, List<(string, string)> blocks)
    {
        ClearLessonBoard();
        currentLessonID = itemID;

        foreach (var pair in blocks)
        {
            GameObject blockGO = Instantiate(blockPrefab, blockArea);
            LessonBlock block = blockGO.GetComponent<LessonBlock>();
            block.statementText = pair.Item1;
            block.label.text = pair.Item2;
            block.originalParent = blockArea;
        }

        for (int i = 0; i < blocks.Count; i++)
        {
            GameObject slot = Instantiate(dropSlotPrefab, dropArea);
            slot.GetComponent<DropSlot>().isTerminalSlot = true;
        }

        lessonBoardPanel.SetActive(true);
        closeButton.SetActive(true);
    }
}
public class LessonValidationRule
{
    public List<string> ExpectedValues; // What we want to match
    public bool UseParsedOutput;        // Whether to use ParseLine() result

    public LessonValidationRule(List<string> values, bool useParsed = false)
    {
        ExpectedValues = values;
        UseParsedOutput = useParsed;
    }
}

