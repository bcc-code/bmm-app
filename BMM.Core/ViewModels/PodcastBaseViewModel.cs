using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public abstract class PodcastBaseViewModel : LoadMoreDocumentsViewModel
    {
        private Podcast _podcast;

        public PodcastBaseViewModel(IDocumentFilter documentFilter = null)
            : base(documentFilter)
        {
            _podcast = new Podcast
            {
                Title = ""
            };

            TrackInfoProvider = new AudiobookPodcastInfoProvider(TrackInfoProvider);
        }

        public Podcast Podcast
        {
            get => _podcast;
            set
            {
                SetProperty(ref _podcast, value);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Image);
            }
        }

        public string Title => Podcast.Title;
        public string Image => Podcast.Cover;
    }
}

