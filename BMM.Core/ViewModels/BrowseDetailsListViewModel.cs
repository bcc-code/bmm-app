using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels
{
    public class BrowseDetailsListViewModel : LoadMoreDocumentsViewModel, IBrowseDetailsViewModel
    {
        private readonly IDocumentsPOFactory _documentsPOFactory;
        private string _path;
        private string _title = StringConstants.Space;

        public BrowseDetailsListViewModel(IDocumentsPOFactory documentsPOFactory, ITrackPOFactory trackPOFactory) : base(trackPOFactory)
        {
            _documentsPOFactory = documentsPOFactory;
        }
        
        public string Title
        {
            get => _title;
            private set => SetProperty(ref _title, value);
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            var response = await Client.Browse.GetDocuments(_path, startIndex, size);
            Title = TextSource.GetTranslationsSafe(response.GetTranslationKey(), response.Title);
            IsFullyLoaded = !response.SupportsPaging;
            return _documentsPOFactory.Create(
                response.Items,
                DocumentSelectedCommand,
                OptionCommand,
                TrackInfoProvider);
        }

        public void Prepare(IBrowseDetailsParameters browseDetailsParameters)
        {
            _path = browseDetailsParameters.Path;
        }
    }
}