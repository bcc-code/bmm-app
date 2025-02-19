using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using Newtonsoft.Json;

namespace BMM.Core.ViewModels;

public class HvheDetailsViewModel : BaseViewModel<IHvheDetailsParameters>, IHvheDetailsViewModel
{ 
    private readonly IStatisticsClient _statisticsClient;
    private HvheChurchesSelectorPO _hvheChurchesSelectorPO;
    private ProjectStandings _standings;

    public HvheDetailsViewModel(IStatisticsClient statisticsClient)
    {
        _statisticsClient = statisticsClient;
        SelectLeftItemCommand = new MvxCommand(() => _hvheChurchesSelectorPO!.LeftItemSelectedCommand.Execute());
        SelectRightItemCommand = new MvxCommand(() => _hvheChurchesSelectorPO!.RightItemSelectedCommand.Execute());
    }

    public IBmmObservableCollection<IBasePO> Items { get; } = new BmmObservableCollection<IBasePO>();
    public IMvxCommand SelectLeftItemCommand { get; }
    public IMvxCommand SelectRightItemCommand { get; }

    public override async Task Initialize()
    {
        await base.Initialize();
        
        _standings = await _statisticsClient.GetStandings();

        _standings.LargeChurches.First().IsHighlighted = true;
        
        _hvheChurchesSelectorPO = new HvheChurchesSelectorPO(
            _standings.LargeChurchesTitle,
            _standings.SmallChurchesTitle,
            SelectionChangedAction);
        
        Items.Add(new HvheBoysVsGirlsPO(
            _standings.BoysTitle,
            _standings.BoysPoints,
            _standings.GirlsTitle,
            _standings.GirlsPoints));
        
        Items.Add(_hvheChurchesSelectorPO);
        
        Items.Add(new HvheHeaderPO(
            _standings.ChurchTitle,
            _standings.GameNightsTitle));
        
        AddLargeChurches();
    }

    private void SelectionChangedAction()
    {
        RemoveChurchItems();
        
        if (_hvheChurchesSelectorPO.IsLeftItemSelected)
            AddLargeChurches();
        else
            AddSmallChurches();
    }

    private void AddLargeChurches()
    {
        var itemsToAdd = _standings
            .LargeChurches
            .Select(church => new HvheChurchPO(church))
            .ToList();
        Items.AddRange(itemsToAdd);
    }
    
    private void AddSmallChurches()
    {
        var itemsToAdd = _standings
            .SmallChurches
            .Select(church => new HvheChurchPO(church))
            .ToList();
        Items.AddRange(itemsToAdd);
    }

    private void RemoveChurchItems()
    {
        var itemsToRemove = Items
            .OfType<HvheChurchPO>()
            .ToList();
        Items.RemoveItems(itemsToRemove);
    }
}