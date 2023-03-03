using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Models.POs;
using BMM.Core.Models.POs.Base;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class ChangeTrackLanguageViewModel : BaseViewModel<ITrackModel>, IChangeTrackLanguageViewModel
    {
        private readonly IPrepareAvailableTrackLanguageAction _prepareAvailableTrackLanguageAction;

        public ChangeTrackLanguageViewModel(
            IPrepareAvailableTrackLanguageAction prepareAvailableTrackLanguageAction,
            ITracksClient tracksClient,
            IMediaPlayer mediaPlayer,
            IAnalytics analytics)
        {
            _prepareAvailableTrackLanguageAction = prepareAvailableTrackLanguageAction;
            _prepareAvailableTrackLanguageAction.AttachDataContext(this);
            
            LanguageSelectedCommand = new ExceptionHandlingCommand<BasePO>(async lang =>
            {
                if (lang is StandardSelectablePO selectedLanguage)
                    SelectTheme(selectedLanguage);
                
                if (SelectedLanguage == NavigationParameter.Language)
                    return;
            
                var track = await tracksClient.GetById(mediaPlayer.CurrentTrack.Id, SelectedLanguage);
                await mediaPlayer.ReplaceTrack(track);
                analytics.LogEvent(Event.TrackLanguageChanged);
                await NavigationService.Close(this);
            });
        }

        private string SelectedLanguage => AvailableLanguages.OfType<StandardSelectablePO>().First(x => x.IsSelected).Value;
            
        public IMvxCommand<BasePO> LanguageSelectedCommand { get; }
        
        public IBmmObservableCollection<BasePO> AvailableLanguages { get; } = new BmmObservableCollection<BasePO>();

        public override async Task Initialize()
        {
            await base.Initialize();
            await _prepareAvailableTrackLanguageAction.ExecuteGuarded();
        }
        
        private void SelectTheme(StandardSelectablePO language)
        {
            AvailableLanguages
                .OfType<StandardSelectablePO>()
                .FirstOrDefault(s => s.IsSelected)
                .IfNotNull(s => s.IsSelected = false);

            AvailableLanguages
                .OfType<StandardSelectablePO>()
                .FirstOrDefault(s => s == language)
                .IfNotNull(s => s.IsSelected = true);
        }
    }
}