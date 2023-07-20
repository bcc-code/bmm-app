using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Factories.Streak;
using BMM.Core.Implementations.Storage;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.ViewModels;

public class BibleStudyViewModel : BaseViewModel, IBibleStudyViewModel
{
    private readonly IListeningStreakPOFactory _listeningStreakPOFactory;
    
    public BibleStudyViewModel(IListeningStreakPOFactory listeningStreakPOFactory)
    {
        _listeningStreakPOFactory = listeningStreakPOFactory;
    }
   
    public IBmmObservableCollection<IBasePO> Items { get; } = new BmmObservableCollection<IBasePO>();
    

    public override async Task Initialize()
    {
        await base.Initialize();
        Items.Add(new BibleStudyHeaderPO("Vær et forbilde", "La ikke Satan få stjele deler av livet ditt", "Nov 6, 2022"));
        var streak = _listeningStreakPOFactory.Create(AppSettings.LatestListeningStreak);
        Items.Add(new BibleStudyProgressPO(streak));
    }
}