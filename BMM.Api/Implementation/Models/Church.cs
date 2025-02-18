namespace BMM.Api.Implementation.Models;

public class Church
{
    public Church(string name, bool isHighlighted, int boysPoints, int girlsPoints, IList<string> gameNights)
    {
        Name = name;
        IsHighlighted = isHighlighted;
        BoysPoints = boysPoints;
        GirlsPoints = girlsPoints;
        GameNights = gameNights;
    }

    public string Name { get; }
    public bool IsHighlighted { get; }
    public int BoysPoints { get; }
    public int GirlsPoints { get; }
    public IList<string> GameNights { get; }
}