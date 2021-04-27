using System;
using System.Web;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.DeepLinking
{
    public class TrackLinkParser : IDeepLinkParser
    {
        private readonly Func<int, string, long, Task> _actionWithParameters;
        private readonly string _regex;

        public TrackLinkParser(string regex, Func<int, string, long, Task> actionWithParameters)
        {
            _actionWithParameters = actionWithParameters;
            _regex = regex;
        }

        public bool CanNavigateTo(Uri uri, out Func<Task> action)
        {
            var regexHandler = new RegexDeepLinkWithParameters(_regex,
                (id, name) =>
                {
                    var startTimeInMs = GetTimeFromUri(uri);
                    return _actionWithParameters.Invoke(id, name, startTimeInMs);
                });
            return regexHandler.CanNavigateTo(uri, out action);
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
