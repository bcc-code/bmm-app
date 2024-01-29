using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation;

public class AchievementsHolder
{
    public string Title { get; set; }
    public IEnumerable<Document> Items { get; set; }
}