using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters
{
    public class BrowseDetailsParameters : IBrowseDetailsParameters
    {
        public BrowseDetailsParameters(BrowseDetailsType browseDetailsType, string title)
        {
            BrowseDetailsType = browseDetailsType;
            Title = title;
        }

        public BrowseDetailsType BrowseDetailsType { get; }
        public string Title { get; }
    }
}