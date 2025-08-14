using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivityQuestion
{
    public string question;
    public List<string> acceptedAnswers;
    public string pseudoOutputIfCorrect;
}

public class Item : MonoBehaviour
{
    public int ID;
    public string itemName;

    [TextArea(2, 6)]
    public string description;

    [TextArea(2, 6)]
    public string syntaxExplanation;

    public Sprite icon;

    public List<ActivityQuestion> activityQuestions = new List<ActivityQuestion>();
}