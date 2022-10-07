using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Factories.Tracks;

namespace BMM.Core.ViewModels
{
    public class BrowseDetailsTilesViewModel : BrowseDetailsListViewModel
    {
        public BrowseDetailsTilesViewModel(IDocumentsPOFactory documentsPOFactory, ITrackPOFactory trackPOFactory) : base(documentsPOFactory, trackPOFactory)
        {
        }
    }
}