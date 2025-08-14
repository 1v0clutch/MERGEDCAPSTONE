// QuestionData.cs - Handles all question-related data and operations
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string prompt;
    public string answer;
    public bool isCodeQuestion;
    public string expectedOutput;
    public string requiredKeyword;
    public string buggyCode;
}

public class QuestionData : MonoBehaviour
{
    public List<Question> timerQuestions = new List<Question>();
    public List<Question> codeQuestions = new List<Question>();
    public List<Question> qnaQuestions = new List<Question>();
    public List<Question> challengeQuestions = new List<Question>();
    public List<Question> debugQuestions = new List<Question>();

    private string[] brokenCodes = new string[]
    {
        "int x = 5\nSystem.out.println(x);",
        "for(int i = 0 i < 5; i++) {\nSystem.out.println(i);\n}",
        "int[] nums = {1, 2, 3};\nSystem.out.println(nums[3]);"
    };

    public void Initialize()
    {
        LoadQuestions();
        InitializeTimerQuestions();
    }

    private void LoadQuestions()
    {
        // Q&A questions
        qnaQuestions.Add(new Question { prompt = "What keyword declares an integer in Java?", answer = "int", isCodeQuestion = false });
        qnaQuestions.Add(new Question { prompt = "Which keyword creates a constant in Java?", answer = "final", isCodeQuestion = false });
        qnaQuestions.Add(new Question { prompt = "What keyword is used for conditional branching?", answer = "if", isCodeQuestion = false });
        qnaQuestions.Add(new Question { prompt = "Which keyword defines a loop that runs while a condition is true?", answer = "while", isCodeQuestion = false });
        qnaQuestions.Add(new Question { prompt = "What symbol ends a Java statement?", answer = ";", isCodeQuestion = false });

        // Basic coding challenges
        codeQuestions.Add(new Question { prompt = "Print 'Hello, World!' in Java.", isCodeQuestion = true, expectedOutput = "Hello, World!\n" });
        codeQuestions.Add(new Question { prompt = "Write Java code to print the sum of 5 and 7.", isCodeQuestion = true, expectedOutput = "12\n" });
        codeQuestions.Add(new Question { prompt = "Print all numbers from 1 to 3, one per line.", isCodeQuestion = true, expectedOutput = "1\n2\n3\n" });
        codeQuestions.Add(new Question { prompt = "Print 'Even' if 4 is divisible by 2.", isCodeQuestion = true, expectedOutput = "Even\n" });
        codeQuestions.Add(new Question { prompt = "Write code to print the result of 3 * 4.", isCodeQuestion = true, expectedOutput = "12\n" });

        // Advanced challenges
        challengeQuestions.Add(new Question { prompt = "Using a for loop, print numbers 0 to 2.", isCodeQuestion = true, expectedOutput = "0\n1\n2\n", requiredKeyword = "for" });
        challengeQuestions.Add(new Question { prompt = "Check if 9 > 3, and print 'True' if it is.", isCodeQuestion = true, expectedOutput = "True\n" });
        challengeQuestions.Add(new Question { prompt = "Declare a=5, b=7, print their sum.", isCodeQuestion = true, expectedOutput = "12\n" });

        // Debugging exercises
        debugQuestions.Add(new Question
        {
            prompt = "Fix the code: Missing semicolon.",
            buggyCode = "int x = 5\nSystem.out.println(x);",
            answer = "int x = 5;\nSystem.out.println(x);",
            isCodeQuestion = true,
            expectedOutput = "5\n"
        });

        debugQuestions.Add(new Question
        {
            prompt = "Fix the code: For loop syntax error.",
            buggyCode = "for(int i = 0 i < 5; i++) {\nSystem.out.println(i);\n}",
            answer = "for(int i = 0; i < 5; i++) {\nSystem.out.println(i);\n}",
            isCodeQuestion = true,
            expectedOutput = "0\n1\n2\n3\n4\n"
        });
    }

    private void InitializeTimerQuestions()
    {
        if (timerQuestions.Count == 0)
        {
            timerQuestions.Add(new Question
            {
                prompt = "Fix ALL bugs in 30 seconds! (2 errors)",
                buggyCode = "public class Main {\n  public static void main(String args) {\n    int x = 5\n    System.out.println(x)\n  }\n}",
                answer = "public class Main {\n  public static void main(String[] args) {\n    int x = 5;\n    System.out.println(x);\n  }\n}",
                isCodeQuestion = true,
                expectedOutput = "5\n"
            });

            timerQuestions.Add(new Question
            {
                prompt = "Fix the loop in 30 seconds!",
                buggyCode = "for(int i=0 i<3;i++){\nSystem.out.println(i)\n}",
                answer = "for(int i=0; i<3; i++){\nSystem.out.println(i);\n}",
                isCodeQuestion = true,
                expectedOutput = "0\n1\n2\n"
            });
        }
    }

    public string GetRandomBrokenCode()
    {
        return brokenCodes[Random.Range(0, brokenCodes.Length)];
    }
}