namespace BMM.Api.Implementation.Models;

public class ShortAnswer
{
    public ShortAnswer(string id, string text, bool hasPrimaryStyle)
    {
        Id = id;
        Text = text;
        HasPrimaryStyle = hasPrimaryStyle;
    }
    
    public string Id { get; }
    public string Text { get; }
    public bool HasPrimaryStyle { get; }
}