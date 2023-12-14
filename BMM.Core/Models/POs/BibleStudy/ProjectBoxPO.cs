using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.Parameters;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.BibleStudy;

public class ProjectBoxPO : DocumentPO, IProjectBoxPO
{
    private readonly IMvxNavigationService _navigationService;

    public ProjectBoxPO(ProjectBox projectBox, IMvxNavigationService navigationService) : base(projectBox)
    {
        _navigationService = navigationService;
        ExpandOrCollapseInteraction = new BmmInteraction();
        ProjectBox = projectBox;
        ExpandOrCollapseCommand = new MvxCommand(() =>
        {
            IsExpanded = !IsExpanded;
            ExpandOrCollapseInteraction?.Raise();
        });
        
        foreach (var achievement in projectBox.Achievements)
            Achievements.Add(new AchievementPO(achievement, navigationService));

        OpenQuestionsCommand = new ExceptionHandlingCommand(async () =>
            await _navigationService.Navigate<WebBrowserViewModel, IWebBrowserPrepareParams>(new WebBrowserPrepareParams
            {
                Url = ProjectBox.ButtonWebsite,
                Title = ProjectBox.ButtonTitle
            }));
        OpenRulesCommand = new ExceptionHandlingCommand(async () =>
        {
            await navigationService.Navigate<BibleStudyRulesViewModel, int>(ProjectBox.Id);
        });
    }
    
    public ProjectBox ProjectBox { get; }
    public bool IsExpanded { get; private set; }
    public IBmmInteraction ExpandOrCollapseInteraction { get; }
    public IMvxCommand ExpandOrCollapseCommand { get; }
    public IMvxAsyncCommand OpenQuestionsCommand { get; }
    public IMvxAsyncCommand OpenRulesCommand { get; set; }
    
    public IBmmObservableCollection<IAchievementPO> Achievements { get; } = new BmmObservableCollection<IAchievementPO>();
}