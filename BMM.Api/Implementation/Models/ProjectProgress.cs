using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Models;

public class ProjectProgress
{
    public ListeningStreak Streak { get; set; }
    public int Days { get; set; }
    public int CurrentBoost { get; set; }
    public int Points { get; set; }
    public string RulesHtmlUrl { get; set; }
    public IList<Achievement> Achievements { get; set; }
}