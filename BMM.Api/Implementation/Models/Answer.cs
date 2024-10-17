namespace BMM.Api.Implementation.Models;

public class Answer
{
    public Answer(string id, string text, bool isCorrect)
    {
        Id = id;
        Text = text;
        IsCorrect = isCorrect;
    }
    
    public string Id { get; }
    public string Text { get; }
    public bool IsCorrect { get; }
}