using System.Drawing;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.ListeningStreaks;

namespace BMM.Core.Models.POs.BibleStudy;

public class BibleStudyProgressPO : BasePO, IBibleStudyProgressPO
{
    private ListeningStreakPO _listeningStreak;

    public BibleStudyProgressPO(ListeningStreakPO listeningStreakPO)
    {
        ListeningStreakPO = listeningStreakPO;
        BoostNumber = "2x";
        DaysNumber = "43";
        PointsNumber = "11";
    }
    
    public ListeningStreakPO ListeningStreakPO
    {
        get => _listeningStreak;
        set => SetProperty(ref _listeningStreak, value);
    }

    private Color GetColor(bool? active)
    {
        if (active == true)
            return ColorTranslator.FromHtml("#AB90FF");

        return Color.Transparent;
    }
    
    public Color MondayColor => GetColor(_listeningStreak.ListeningStreak.Monday);
    public Color TuesdayColor => GetColor(true);
    public Color WednesdayColor => GetColor(_listeningStreak.ListeningStreak.Wednesday);
    public Color ThursdayColor => GetColor(true);
    public Color FridayColor => GetColor(_listeningStreak.ListeningStreak.Friday);
    public string DaysNumber { get; }
    public string BoostNumber { get; }
    public string PointsNumber { get; }
}