namespace BMM.Api.Implementation.Models;

public class Question
{
    public Question(int id,
        string type,
        string pageTitle,
        string questionImageLink,
        string questionText,
        string questionSubtext,
        string linkUrl,
        string linkText,
        string solutionTextPlaceholder,
        string solutionTextCorrect,
        string solutionTextWrong,
        string style,
        List<Answer> answers,
        List<ShortAnswer> shortAnswers)
    {
        Id = id;
        Type = type;
        PageTitle = pageTitle;
        QuestionImageLink = questionImageLink;
        QuestionText = questionText;
        QuestionSubtext = questionSubtext;
        LinkUrl = linkUrl;
        LinkText = linkText;
        SolutionTextPlaceholder = solutionTextPlaceholder;
        SolutionTextCorrect = solutionTextCorrect;
        SolutionTextWrong = solutionTextWrong;
        Style = style;
        Answers = answers;
        ShortAnswers = shortAnswers;
    }

    public int Id { get; }
    public string Type { get; }
    public string PageTitle { get; }
    public string QuestionImageLink { get; }
    public string QuestionText { get; }
    public string QuestionSubtext { get; }
    public string LinkUrl { get; }
    public string LinkText { get; }
    public string SolutionTextPlaceholder { get; }
    public string SolutionTextCorrect { get; }
    public string SolutionTextWrong { get; }
    public string Style { get; }
    public List<Answer> Answers { get; }
    public List<ShortAnswer> ShortAnswers { get; }
}