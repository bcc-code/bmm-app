using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Models;

public class Church
{
    public Church(string name, bool isHighlighted, int boysPoints, int girlsPoints, IList<GameNights> gameNights)
    {
        Name = name;
        IsHighlighted = isHighlighted;
        BoysPoints = boysPoints;
        GirlsPoints = girlsPoints;
        GameNights = gameNights;
    }

    public string Name { get; set; }
    public bool IsHighlighted { get; set; }
    public int BoysPoints { get; set; }
    public int GirlsPoints { get; set; }
    public IList<GameNights> GameNights { get; set; }
}