namespace BMM.Core.Models.Parameters
{
    public class WebBrowserPrepareParams : IWebBrowserPrepareParams
    {
        private string _url;
        public WebBrowserPrepareParams() { }

        private WebBrowserPrepareParams(string title, string url)
        {
            Title = title;
            _url = url;
        }

        public string Url
        {
            get => _url;
            set => _url = value;
        }

        public string Title { get; set; }
        public IDictionary<string, Action<string>> JavaScriptEventHandlers { get; set; }

        public static WebBrowserPrepareParams CreateFromLink(string url, string title)
            => new WebBrowserPrepareParams(title, url);
    }
}