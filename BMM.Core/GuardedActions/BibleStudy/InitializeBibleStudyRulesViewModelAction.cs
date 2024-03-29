using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.GuardedActions.TrackInfo.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Factories.Streak;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.Other;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Interfaces;
using Microsoft.Maui.Devices;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.BibleStudy;

public class InitializeBibleStudyRulesViewModelAction : GuardedActionWithParameter<int>, IInitializeBibleStudyRulesViewModelAction
{
    private readonly IStatisticsClient _statisticsClient;

    private IBibleStudyRulesViewModel DataContext => this.GetDataContext();

    public InitializeBibleStudyRulesViewModelAction(IStatisticsClient statisticsClient)
    {
        _statisticsClient = statisticsClient;
    }
    
    protected override async Task Execute(int projectId)
    {
        var projectRules = await _statisticsClient.GetProjectRules(projectId);
        var items = new List<IBasePO>();
        
        DataContext.Title = projectRules.PageTitle;
        
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            items.Add(new BibleStudyRulesHeaderPO(projectRules.PageTitle));

        foreach (var section in projectRules.Sections)
            items.Add(new BibleStudyRulesContentPO(section.Title, section.Text));

        DataContext.Items.AddRange(items);
    }
}