using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels
{
    public class BrowseDetailsViewModel : LoadMoreDocumentsViewModel, IBrowseDetailsViewModel
    {
        private BrowseDetailsType _browseDetailsType;

        public override async Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            switch (_browseDetailsType)
            {
                case BrowseDetailsType.Audiobooks:
                    return await Client.Browse.GetAudiobooks(startIndex, size);
                case BrowseDetailsType.Events:
                    return await Client.Browse.GetEvents(startIndex, size);
                case BrowseDetailsType.Music:
                    return await Client.Browse.GetMusic();
                case BrowseDetailsType.Podcasts:
                    return await Client.Browse.GetPodcasts();
                default:
                    return default;
            }
        }

        public void Prepare(IBrowseDetailsParameters browseDetailsParameters)
        {
            _browseDetailsType = browseDetailsParameters.BrowseDetailsType;
            Title = browseDetailsParameters.Title;
        }

        public string Title { get; private set; }
    }
}