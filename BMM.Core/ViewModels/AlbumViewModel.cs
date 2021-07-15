using System;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Core.Helpers;
using BMM.Core.Implementations.TrackInformation.Strategies;
using MvvmCross.Commands;
using MvvmCross.Localization;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class AlbumViewModel : DocumentsViewModel, IMvxViewModel<int>, IMvxViewModel<Album>, ITrackListViewModel
    {
        private int _id;

        /// <summary>
        /// While iOS reuses <see cref="BaseViewModel.OptionsAction"/> Android creates a new menu using these commands.
        /// </summary>
        public IMvxCommand AddToPlaylistCommand { get; }

        public IMvxCommand ShareCommand { get; }

        private Album _album;
        public Album Album
        {
            get => _album;
            set
            {
                SetProperty(ref _album, value);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => Description);
                RaisePropertyChanged(() => Image);
                RaisePropertyChanged(() => ShowImage);
            }
        }

        public AlbumViewModel(IShareLink shareLink)
        {
            AddToPlaylistCommand = new ExceptionHandlingCommand(async () => await AddAlbumToTrackCollection(Album.Id));
            ShareCommand = new ExceptionHandlingCommand(async () => await shareLink.For(_album));

            var audiobookStyler = new AudiobookPodcastInfoProvider(TrackInfoProvider);
            TrackInfoProvider = new CustomTrackInfoProvider(TrackInfoProvider,
                (track, culture, defaultTrack) =>
                {
                    if (audiobookStyler.HasSpecificStyling(track))
                        return audiobookStyler.GetTrackInformation(track, culture, defaultTrack);

                    if (track.Subtype == TrackSubType.Speech || track.Subtype == TrackSubType.Exegesis)
                    {
                        return new TrackInformation
                        {
                            Label = track.Artist,
                            Subtitle = defaultTrack.Subtitle,
                            Meta = string.IsNullOrWhiteSpace(track.Title) || track.Title == track.Artist ? ReadableDuration(track.Duration) : track.Title
                        };
                    }

                    return new TrackInformation
                    {
                        Label = track.Title,
                        Subtitle = defaultTrack.Subtitle,
                        Meta = string.IsNullOrWhiteSpace(track.Artist) || track.Artist == track.Title ? ReadableDuration(track.Duration) : track.Artist
                    };
                });

            Documents.CollectionChanged += UpdateView;
        }

        public string ReadableDuration(long durationInMilliseconds)
        {
            var span = TimeSpan.FromMilliseconds(durationInMilliseconds);
            var seconds = span.Seconds;
            var minutes = Math.Floor(span.TotalMinutes);
            return $"{minutes} minutes {seconds} seconds";
        }

        /// <summary>
        /// In case we already have a title we can show the title immediately without waiting for <see cref="LoadItems"/>.
        /// </summary>
        public void Prepare(Album album)
        {
            _id = album.Id;
            Album = album;
        }

        public void Prepare(int id)
        {
            _id = id;
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            Album = await Client.Albums.GetById(_id);
            return Album?.Children;
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            Documents.CollectionChanged -= UpdateView;
            base.ViewDestroy(viewFinishing);
        }

        private void UpdateView(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => ShowShuffleButton);
        }

        public bool ShowSharingInfo => false;

        public bool ShowDownloadButtons => false;

        public bool IsDownloaded => false;

        public string Title => Album?.Title;

        public string Description => Album?.Description;

        public bool ShowPlaylistIcon => false;
        public bool ShowImage => Album?.Cover != null;
        public string Image => Album?.Cover;
        public bool UseCircularImage => false;

        public bool ShowFollowButtons => false;

        public bool ShowShuffleButton => Documents.OfType<Track>().Any();
        public bool ShowPlayButton => false;

        public bool ShowTrackCount => true;

        public bool ShowFollowSharedPlaylistButton => false;

        public override string TrackCountString => Documents.OfType<Album>().Any() ? TextSource.GetText("PluralAlbums", Documents.OfType<Album>().Count()) : base.TrackCountString;
    }
}