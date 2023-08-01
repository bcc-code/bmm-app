using BMM.Core.Extensions;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels;

public class BibleStudyViewModel : BaseViewModel<IBibleStudyParameters>, IBibleStudyViewModel
{
    private readonly IInitializeBibleStudyViewModelAction _initializeBibleStudyViewModelAction;

    public BibleStudyViewModel(
        IInitializeBibleStudyViewModelAction initializeBibleStudyViewModelAction)
    {
        _initializeBibleStudyViewModelAction = initializeBibleStudyViewModelAction;
        _initializeBibleStudyViewModelAction.AttachDataContext(this);
    }
   
    public IBmmObservableCollection<IBasePO> Items { get; } = new BmmObservableCollection<IBasePO>();
    
    public override async Task Initialize()
    {
        await base.Initialize();
        await _initializeBibleStudyViewModelAction.ExecuteGuarded();
    }
}