using System.Drawing;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations;
using BMM.Core.Models.Parameters;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Models.POs.ListeningStreaks;
using BMM.Core.ViewModels;
using Microsoft.Maui.Devices.Sensors;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.BibleStudy;

public class BibleStudyProgressPO : BasePO, IBibleStudyProgressPO
{
    private ListeningStreakPO _listeningStreak;

    public BibleStudyProgressPO(
        ListeningStreakPO streak,
        ProjectProgress projectProgress,
        IMvxNavigationService navigationService)
    {
        ListeningStreakPO = streak;
        DaysNumber = projectProgress.Days.ToString();
        PointsNumber = projectProgress.Points.ToString();
        BoostNumber = $"{projectProgress.CurrentBoost}x";

        foreach (var achievement in projectProgress.Achievements)
            Achievements.Add(new AchievementPO(achievement, navigationService));

        TermsButtonClickedCommand = new ExceptionHandlingCommand(async () =>
        {
            await navigationService.Navigate<BibleStudyRulesViewModel>();
        });
        TermsWebButtonClickedCommand = new ExceptionHandlingCommand(async () =>
        {
            await navigationService.Navigate<WebBrowserViewModel, IWebBrowserPrepareParams>(new WebBrowserPrepareParams
            {
                Url = "https://google.com"
            });
        });
    }

    public ListeningStreakPO ListeningStreakPO
    {
        get => _listeningStreak;
        set => SetProperty(ref _listeningStreak, value);
    }

    private string GetColor(bool? active)
    {
        if (active == true)
            return ListeningStreakPO.ListeningStreak.PointColor;

        return null;
    }
    
    public string MondayColor => GetColor(_listeningStreak.ListeningStreak.Monday);
    public string TuesdayColor => GetColor(_listeningStreak.ListeningStreak.Tuesday);
    public string WednesdayColor => GetColor(_listeningStreak.ListeningStreak.Wednesday);
    public string ThursdayColor => GetColor(_listeningStreak.ListeningStreak.Thursday);
    public string FridayColor => GetColor(_listeningStreak.ListeningStreak.Friday);
    public string DaysNumber { get; }
    public string BoostNumber { get; }
    public string PointsNumber { get; }
    public IBmmObservableCollection<IAchievementPO> Achievements { get; } = new BmmObservableCollection<IAchievementPO>();
    public IMvxAsyncCommand TermsButtonClickedCommand { get; }
    public IMvxAsyncCommand TermsWebButtonClickedCommand { get; }
}