namespace BMM.Api.Implementation.Models;

public class ProjectBox : Document
{
    public ProjectBox(DocumentType documentType)
    {
        DocumentType = documentType;
    }
    
    public string Title { get; set; }
    public bool ShowIcon { get; set; }
    public bool OpenByDefault { get; set; }
    public int Points { get; set; }
    public string PointsDescription { get; set; }
    public string ButtonTitle { get; set; }
    public string ButtonWebsite { get; set; }
    public bool ButtonShowIcon { get; set; }
    public IList<Achievement> Achievements { get; set; }
    public string RulesLinkTitle { get; set; }
    public string IconColor { get; set; }
}