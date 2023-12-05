using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.BibleStudy;

public class ProjectBoxPO : DocumentPO, IProjectBoxPO
{
    public ProjectBoxPO(ProjectBox projectBox, IMvxNavigationService navigationService) : base(projectBox)
    {
        ExpandOrCollapseInteraction = new BmmInteraction();
        ProjectBox = projectBox;
        ExpandOrCollapseCommand = new MvxCommand(() =>
        {
            IsExpanded = !IsExpanded;
            ExpandOrCollapseInteraction?.Raise();
        });
        
        foreach (var achievement in projectBox.Achievements)
            Achievements.Add(new AchievementPO(achievement, navigationService));
    }
    
    public ProjectBox ProjectBox { get; }
    public bool IsExpanded { get; private set; }
    public IBmmInteraction ExpandOrCollapseInteraction { get; }
    public IMvxCommand ExpandOrCollapseCommand { get; }
    public IBmmObservableCollection<IAchievementPO> Achievements { get; } = new BmmObservableCollection<IAchievementPO>();
}