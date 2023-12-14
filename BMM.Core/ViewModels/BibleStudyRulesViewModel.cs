using BMM.Core.Extensions;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels;

public class BibleStudyRulesViewModel : BaseViewModel<IBibleStudyParameters>, IBibleStudyRulesViewModel, IMvxViewModel<int>
{
    private readonly IInitializeBibleStudyRulesViewModelAction _initializeBibleStudyRulesViewModelAction;
    private string _title;
    private int _projectId;

    public BibleStudyRulesViewModel(IInitializeBibleStudyRulesViewModelAction initializeBibleStudyRulesViewModelAction)
    {
        _initializeBibleStudyRulesViewModelAction = initializeBibleStudyRulesViewModelAction;
        _initializeBibleStudyRulesViewModelAction.AttachDataContext(this);
    }
   
    public IBmmObservableCollection<IBasePO> Items { get; } = new BmmObservableCollection<IBasePO>();

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public void Prepare(int projectId)
    {
        _projectId = projectId;
    }

    public override async Task Initialize()
    {
        await base.Initialize();
        await _initializeBibleStudyRulesViewModelAction.ExecuteGuarded(_projectId);
    }
}