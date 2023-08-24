using System.Drawing;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IBibleStudyProgressPO : IBasePO
{
    string MondayColor { get; }
    string TuesdayColor { get; }
    string WednesdayColor { get; }
    string ThursdayColor { get; }
    string FridayColor { get; }
    string DaysNumber { get; }
    string BoostNumber { get; }
    string PointsNumber { get; }
    IBmmObservableCollection<IAchievementPO> Achievements { get; }
    IMvxAsyncCommand TermsButtonClickedCommand { get; }
}