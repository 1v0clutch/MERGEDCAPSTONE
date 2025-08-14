using System.Collections.Generic;
using UnityEngine; 

public class QuestionSelector
{
    private QuestionData questionData;

    public QuestionSelector(QuestionData data)
    {
        questionData = data;
    }

    public List<Question> GetSelectedQuestions()
    {
        var selected = new List<Question>();
        for (int i = 0; i < 3; i++)
        {
            selected.Add(questionData.codeQuestions[Random.Range(0, questionData.codeQuestions.Count)]);
            selected.Add(questionData.qnaQuestions[Random.Range(0, questionData.qnaQuestions.Count)]);
            selected.Add(questionData.challengeQuestions[Random.Range(0, questionData.challengeQuestions.Count)]);
            selected.Add(questionData.debugQuestions[Random.Range(0, questionData.debugQuestions.Count)]);
        }
        return selected;
    }
}
