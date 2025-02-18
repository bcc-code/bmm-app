using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels;

public class HvheDetailsViewModel : BaseViewModel<IHvheDetailsParameters>, IHvheDetailsViewModel
{
    private readonly IStatisticsClient _statisticsClient;

    public HvheDetailsViewModel(IStatisticsClient statisticsClient)
    {
        _statisticsClient = statisticsClient;
    }

    public IBmmObservableCollection<IBasePO> Items { get; } = new BmmObservableCollection<IBasePO>();

    public override async Task Initialize()
    {
        await base.Initialize();
        var standings = await _statisticsClient.GetStandings();
        
        Items.Add(new HvheBoysVsGirlsPO(
            standings.BoysTitle,
            standings.BoysPoints,
            standings.GirlsTitle,
            standings.GirlsPoints));
        
        Items.Add(new HvheChurchesSelectorPO(
            standings.LargeChurchesTitle,
            standings.SmallChurchesTitle,
            new List<IBasePO>()));
        
        Items.Add(new HvheHeaderPO(
            standings.ChurchTitle,
            standings.GameNightsTitle));
    }
}