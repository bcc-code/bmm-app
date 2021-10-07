using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters
{
    public class BrowseDetailsParameters : IBrowseDetailsParameters
    {
        public BrowseDetailsParameters(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}