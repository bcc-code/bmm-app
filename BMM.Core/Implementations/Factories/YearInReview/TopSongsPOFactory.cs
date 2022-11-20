using System.Globalization;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.Translation;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.YearInReview
{
    public class TopSongsPOFactory : ITopSongsPOFactory
    {
        private readonly ITrackPOFactory _trackPOFactory;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public TopSongsPOFactory(
            ITrackPOFactory trackPOFactory,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _trackPOFactory = trackPOFactory;
            _bmmLanguageBinder = bmmLanguageBinder;
        }

        public ITrackPO Create(TopSong topSong, IMvxAsyncCommand<Document> optionsClickedCommand)
        {
            return _trackPOFactory.Create(new TopSongsProvider(_bmmLanguageBinder, topSong.PlayCount), optionsClickedCommand, topSong.Track);
        }
    }
    
    public class TopSongsProvider : ITrackInfoProvider
    {
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly int _topSongPlayCount;

        public TopSongsProvider(IBMMLanguageBinder bmmLanguageBinder, int topSongPlayCount)
        {
            _bmmLanguageBinder = bmmLanguageBinder;
            _topSongPlayCount = topSongPlayCount;
        }

        public TrackInformation.Strategies.TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture)
        {
            var defaultTrackInfo = new DefaultTrackInfoProvider().GetTrackInformation(track, culture);
            
            return new TrackInformation.Strategies.TrackInformation()
            {
                Label = defaultTrackInfo.Label,
                Meta = defaultTrackInfo.Meta,
                Subtitle = GetPlaysCount()
            };
        }

        private string GetPlaysCount()
        {
            string key = _topSongPlayCount == 1
                ? Translations.TopSongsCollectionViewModel_PlaysSingular
                : Translations.TopSongsCollectionViewModel_PlaysPlural;
            
            return _bmmLanguageBinder.GetText(key, _topSongPlayCount);
        }
    }
}