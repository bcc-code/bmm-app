namespace BMM.Api.Implementation.Models;

public class PostAnswer
{
    public int QuestionId { get; set; }
    public string InitialAnswerId { get; set; }
    public string FinalAnswerId { get; set; }
    public int Tries { get; set; }
}