namespace BMM.Api.Implementation.Models;

public class Transcription
{
    public int Id { get; set; }
    public decimal Start { get; set; }
    public decimal End { get; set; }
    public string Text { get; set; }
}