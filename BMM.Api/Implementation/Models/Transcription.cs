namespace BMM.Api.Implementation.Models;

public class Transcription
{
    public int Id { get; set; }
    public double Start { get; set; }
    public double End { get; set; }
    public string Text { get; set; }
}