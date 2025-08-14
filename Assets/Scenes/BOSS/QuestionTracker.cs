public class QuestionTracker
{
    public bool[] answered;
    public int totalAnswered;
    public int currentIndex = -1;

    public void Initialize(int count)
    {
        answered = new bool[count];
        totalAnswered = 0;
        currentIndex = -1;
    }

    public bool IsAnswered(int index) => answered[index];
    public void MarkAnswered(int index)
    {
        answered[index] = true;
        totalAnswered++;
        currentIndex = -1;
    }
}