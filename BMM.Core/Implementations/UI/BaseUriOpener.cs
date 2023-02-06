using System;
using System.Collections.Generic;
using BMM.Core.Implementations.Analytics;

namespace BMM.Core.Implementations.UI
{
    public abstract class BaseUriOpener : IUriOpener
    {
        private const string UrlParameterKey = "Url";
        private readonly IAnalytics _analytics;

        protected BaseUriOpener(IAnalytics analytics)
        {
            _analytics = analytics;
        }
        
        public void OpenUri(Uri uri)
        {
            _analytics.LogEvent(Event.NavigateToExternalLink, new Dictionary<string, object>
            {
                {UrlParameterKey, uri.ToString()}
            });
            
            PlatformOpenUri(uri);
        }

        protected abstract void PlatformOpenUri(Uri uri);
    }
}