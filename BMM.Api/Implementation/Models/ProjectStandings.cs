namespace BMM.Api.Implementation.Models;

public class ProjectStandings
{
    public ProjectStandings(string boysTitle,
        string girlsTitle,
        int boysPoints,
        int girlsPoints,
        string largeChurchesTitle,
        string smallChurchesTitle,
        string churchTitle,
        string gameNightsTitle,
        IList<Church> largeChurches,
        IList<Church> smallChurches)
    {
        BoysTitle = boysTitle;
        GirlsTitle = girlsTitle;
        BoysPoints = boysPoints;
        GirlsPoints = girlsPoints;
        LargeChurchesTitle = largeChurchesTitle;
        SmallChurchesTitle = smallChurchesTitle;
        ChurchTitle = churchTitle;
        GameNightsTitle = gameNightsTitle;
        LargeChurches = largeChurches;
        SmallChurches = smallChurches;
    }

    public string BoysTitle { get; set; }
    public string GirlsTitle { get; set; }
    public int BoysPoints { get; set; }
    public int GirlsPoints { get; set; }
    public string LargeChurchesTitle { get; set; }
    public string SmallChurchesTitle { get; set; }
    public string ChurchTitle { get; set; }
    public string GameNightsTitle { get; set; }
    public IList<Church> LargeChurches { get; set; }
    public IList<Church> SmallChurches { get; set; }
}