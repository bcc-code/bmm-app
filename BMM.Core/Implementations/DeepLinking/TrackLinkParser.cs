using System;
using System.Web;
using System.Threading.Tasks;
using BMM.Core.Implementations.DeepLinking.Base;
using BMM.Core.Implementations.DeepLinking.Base.Interfaces;
using BMM.Core.Implementations.DeepLinking.Parameters;

namespace BMM.Core.Implementations.DeepLinking
{
    public class TrackLinkParser : DeepLinkParserBase, IDeepLinkParser
    {
        private readonly Func<TrackLinkParameters, Task> _actionWithParameters;
        private readonly string _regex;

        public TrackLinkParser(string regex, Func<TrackLinkParameters, Task> actionWithParameters)
        {
            _actionWithParameters = actionWithParameters;
            _regex = regex;
        }

        protected override bool CanNavigateTo(Uri uri, out Func<Task> action)
        {
            var regexHandler = new RegexDeepLink<TrackLinkParameters>(_regex,
                (trackLinkParameters) =>
                {
                    trackLinkParameters.StartTimeInMs = GetTimeFromUri(uri);
                    return _actionWithParameters.Invoke(trackLinkParameters);
                });
            return regexHandler.PerformCanNavigateTo(uri, out action);
        }

        private long GetTimeFromUri(Uri uri)
        {
            var timeParamSymbol = "t";
            var startTimeInSeconds = HttpUtility.ParseQueryString(uri.Query).Get(timeParamSymbol);

            if (long.TryParse(startTimeInSeconds, out var result))
            {
                var startTimeInMilliseconds = result * 1000;
                return startTimeInMilliseconds;
            }

            return 0;
        }
    }
}
