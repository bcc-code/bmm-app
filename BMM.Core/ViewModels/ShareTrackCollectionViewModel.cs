using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels
{
    public class ShareTrackCollectionViewModel
        : DocumentsViewModel,
          IShareTrackCollectionViewModel
    {
        private int _trackCollectionId;
        private string _trackCollectionName;
        private string _trackCollectionShareType;

        public string TrackCollectionName
        {
            get => _trackCollectionName;
            private set => SetProperty(ref _trackCollectionName, value);
        }

        public string TrackCollectionShareType
        {
            get => _trackCollectionShareType;
            private set => SetProperty(ref _trackCollectionShareType, value);
        }

        public void Prepare(ITrackCollectionParameter parameter)
        {
            _trackCollectionId = parameter.TrackCollectionId;
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var trackCollection = await Client.TrackCollection.GetById(_trackCollectionId, policy);
            TrackCollectionName = trackCollection.Name;
            TrackCollectionShareType = "Private";
            return trackCollection.Tracks;
        }
    }
}