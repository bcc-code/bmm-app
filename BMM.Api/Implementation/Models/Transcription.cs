namespace BMM.Api.Implementation.Models;

public class Transcription
{
    public Transcription(int id, double start, double end, string text, bool isHeader)
    {
        Id = id;
        Start = start;
        End = end;
        Text = text;
        IsHeader = isHeader;
    }
    
    public int Id { get; }
    public double Start { get; }
    public double End { get; }
    public string Text { get; }
    public bool IsHeader { get; }
}