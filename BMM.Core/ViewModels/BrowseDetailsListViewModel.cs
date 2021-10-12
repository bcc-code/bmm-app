using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels
{
    public class BrowseDetailsListViewModel : LoadMoreDocumentsViewModel, IBrowseDetailsViewModel
    {
        private string _path;
        private string _title = StringConstants.Space;

        public string Title
        {
            get => _title;
            private set => SetProperty(ref _title, value);
        }

        public override async Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            var response = await Client.Browse.GetDocuments(_path, startIndex, size);
            Title = response.Title;
            IsFullyLoaded = !response.SupportsPaging;
            return response.Items;
        }

        public void Prepare(IBrowseDetailsParameters browseDetailsParameters)
        {
            _path = browseDetailsParameters.Path;
        }
    }
}