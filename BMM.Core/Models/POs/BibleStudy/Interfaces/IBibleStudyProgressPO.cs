using System.Drawing;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IBibleStudyProgressPO : IBasePO
{
    Color MondayColor { get; }
    Color TuesdayColor { get; }
    Color WednesdayColor { get; }
    Color ThursdayColor { get; }
    Color FridayColor { get; }
    string DaysNumber { get; }
    string BoostNumber { get; }
    string PointsNumber { get; }
}