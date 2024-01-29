namespace BMM.Api.Implementation.Models;

public class AchievementsCollection : Document
{
    public AchievementsCollection()
    {
        DocumentType = DocumentType.PlaylistsCollection;
    }

    public IEnumerable<Achievement> List { get; set; }
}