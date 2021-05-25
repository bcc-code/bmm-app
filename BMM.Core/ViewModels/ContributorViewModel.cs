using System.Threading.Tasks;
using System.Collections.Generic;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;
using BMM.Core.Implementations.TrackInformation.Strategies;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class ContributorViewModel : LoadMoreDocumentsViewModel, IMvxViewModel<int>, ITrackListViewModel
    {
        private int _id;

        private Contributor _contributor;

        public Contributor Contributor
        {
            get => _contributor;
            private set
            {
                SetProperty(ref _contributor, value);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Image);
            }
        }

        public bool ShowSharingInfo => false;

        public bool ShowImage => true;

        public bool IsDownloadable => false;

        public bool IsDownloaded => false;
        public bool IsOfflineAvailable => false;
        public bool IsDownloading => false;

        public string Title => Contributor?.Name;

        public string Image => Contributor?.Cover;

        public ContributorViewModel()
        {
            TrackInfoProvider = new CustomTrackInfoProvider(TrackInfoProvider,
                (track, culture, defaultTrack) =>
                {
                    return new TrackInformation
                    {
                        Label = string.IsNullOrWhiteSpace(track.Title) ? track.Artist : track.Title,
                        Subtitle = defaultTrack.Subtitle,
                        Meta = defaultTrack.Meta
                    };
                });
        }

        public void Prepare(int contributorId)
        {
            _id = contributorId;
        }

        protected override async Task Initialization()
        {
            await base.Initialization();
            await BackgroundInitialization(LoadContributor);
        }

        private async Task LoadContributor()
        {
            Contributor = await Client.Contributors.GetById(_id);
        }

        public override async Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            return await Client.Contributors.GetTracks(_id, from: startIndex, size: size);
        }
    }
}