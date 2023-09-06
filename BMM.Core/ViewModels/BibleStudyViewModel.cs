using BMM.Core.Extensions;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels;

public class BibleStudyViewModel : BaseViewModel<IBibleStudyParameters>, IBibleStudyViewModel
{
    private readonly IInitializeBibleStudyViewModelAction _initializeBibleStudyViewModelAction;
    private readonly MvxSubscriptionToken _playbackStatusChangedMessageToken;

    public BibleStudyViewModel(
        IInitializeBibleStudyViewModelAction initializeBibleStudyViewModelAction)
    {
        _initializeBibleStudyViewModelAction = initializeBibleStudyViewModelAction;
        _initializeBibleStudyViewModelAction.AttachDataContext(this);
        
        _playbackStatusChangedMessageToken = Messenger.Subscribe<PlaybackStatusChangedMessage>(PlaybackStateChanged);
    }

    private void PlaybackStateChanged(PlaybackStatusChangedMessage obj)
    {
        Items
            .OfType<ITrackHolderPO>()
            .ToList()
            .ForEach(t => t.RefreshState());
    }

    public IBmmObservableCollection<IBasePO> Items { get; } = new BmmObservableCollection<IBasePO>();
    
    public override async Task Initialize()
    {
        await base.Initialize();
        await _initializeBibleStudyViewModelAction.ExecuteGuarded();
    }
}