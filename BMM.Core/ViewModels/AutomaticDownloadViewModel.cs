﻿using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Models;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels
{
    public class AutomaticDownloadViewModel : BaseViewModel, IMvxViewModel<int>
    {
        protected IMvxMessenger Messenger;
        protected MvxSubscriptionToken AutomaticDownloadChangeToken;
        private readonly IPodcastOfflineManager _podcastDownloader;

        public MvxObservableCollection<AutomaticDownloadCellWrapperViewModel> DownloadOptions { get; set; } =
            new MvxObservableCollection<AutomaticDownloadCellWrapperViewModel>();

        public MvxCommand<int> DownloadOptionsSelectedCommand { get; }

        private int _automaticallyDownloadedTracks;

        public int AutomaticallyDownloadedTracks
        {
            get => _automaticallyDownloadedTracks;
            set
            {
                SetProperty(ref _automaticallyDownloadedTracks, value);
                _podcastDownloader.SetNumbeOfTracksToAutomaticallyDownload(Podcast, value);
            }
        }

        private int _podcastId;

        protected Podcast Podcast => new Podcast {Id = _podcastId};

        public AutomaticDownloadViewModel(IPodcastOfflineManager podcastDownloader)
        {
            _podcastDownloader = podcastDownloader;
            Messenger = Mvx.IoCProvider.Resolve<IMvxMessenger>();
            DownloadOptionsSelectedCommand = new MvxCommand<int>(AutomaticDownloadSelected);
        }

        public void Prepare(int podcastId)
        {
            _podcastId = podcastId;
        }

        public override Task Initialize()
        {
            var downloadOptions = new[] {0, 3, 10};

            var cellWrapperViewModels = downloadOptions
                .Select(numberOfTracks => new AutomaticDownloadCellWrapperViewModel(new ValueHeaderItem<int>()
                {
                    Value = numberOfTracks,
                    Title = GetTextForAutomaticDownloadTracks(numberOfTracks)
                }, this));

            DownloadOptions.ReplaceWith(cellWrapperViewModels);
            _automaticallyDownloadedTracks = _podcastDownloader.GetNumberOfTracksToAutomaticallyDownload(Podcast);
            return base.Initialize();
        }

        public string GetTextForAutomaticDownloadTracks(int number)
        {
            if (number == 0)
            {
                return TextSource[Translations.AutomaticDownloadViewModel_None];
            }

            return TextSource.GetText(Translations.AutomaticDownloadViewModel_PluralLatestTracks, number.ToString());
        }

        public void AutomaticDownloadSelected(int selectedItem)
        {
            AutomaticallyDownloadedTracks = selectedItem;
        }
    }
}