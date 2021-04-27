using System;
using System.Globalization;
using BMM.Api.Abstraction;

namespace BMM.Core.Implementations.TrackInformation.Strategies
{
    public class CustomTrackInfoProvider : ITrackInfoProvider
    {
        private readonly ITrackInfoProvider _defaultTrackInfoProvider;
        private readonly Func<ITrackModel, CultureInfo, TrackInformation, TrackInformation> _customFunc;

        public CustomTrackInfoProvider(ITrackInfoProvider defaultTrackInfoProvider, Func<ITrackModel, CultureInfo, TrackInformation, TrackInformation> customFunc)
        {
            _defaultTrackInfoProvider = defaultTrackInfoProvider;
            _customFunc = customFunc;
        }

        public TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture)
        {
            return _customFunc.Invoke(track, culture, _defaultTrackInfoProvider.GetTrackInformation(track, culture));
        }
    }
}