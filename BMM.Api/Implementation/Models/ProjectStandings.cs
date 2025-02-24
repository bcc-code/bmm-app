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

    public string BoysTitle { get; }
    public string GirlsTitle { get; }
    public int BoysPoints { get; }
    public int GirlsPoints { get; }
    public string LargeChurchesTitle { get; }
    public string SmallChurchesTitle { get; }
    public string ChurchTitle { get; }
    public string GameNightsTitle { get; }
    public IList<Church> LargeChurches { get; }
    public IList<Church> SmallChurches { get; }
}