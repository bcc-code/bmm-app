using System.Collections.Generic;
using System.Globalization;
using BMM.Api.Abstraction;

namespace BMM.Core.Implementations.TrackInformation.Strategies
{
    /// <summary>
    /// This is an idea on how to solve more complex scenarios but isn't used yet
    /// </summary>
    public class TrackInfoProviderCombiner : ITrackInfoProvider
    {
        private readonly IList<ISpecificTrackInfoProvider> _providers;
        private readonly ITrackInfoProvider _defaultTrackInfoProvider;

        public TrackInfoProviderCombiner(IList<ISpecificTrackInfoProvider> providers, ITrackInfoProvider defaultTrackInfoProvider)
        {
            this._providers = providers;
            _defaultTrackInfoProvider = defaultTrackInfoProvider;
        }

        public TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture)
        {
            var defaultTrack = _defaultTrackInfoProvider.GetTrackInformation(track, culture);
            foreach (var specificTrackInfoProvider in _providers)
            {
                if (specificTrackInfoProvider.HasSpecificStyling(track))
                {
                    return specificTrackInfoProvider.GetTrackInformation(track, culture, defaultTrack);
                }
            }

            return defaultTrack;
        }
    }
}