using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.UI;
using BMM.Core.Models;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.Localization;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class TrackInfoViewModel : ItemListViewModel, IMvxViewModel<Track>
    {
        private readonly IUriOpener _uriOpener;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IDeepLinkHandler _deepLinkHandler;

        public TrackInfoViewModel(IUriOpener uriOpener, IExceptionHandler exceptionHandler, IDeepLinkHandler deepLinkHandler, IMvxLanguageBinder textSource = null)
        {
            _uriOpener = uriOpener;
            _exceptionHandler = exceptionHandler;
            _deepLinkHandler = deepLinkHandler;
            if (textSource != null)
            {
                TextSource = textSource;
            }
        }

        private Track _track;

        public Track Track
        {
            get => _track;
            set => SetProperty(ref _track, value);
        }

        public void Prepare(Track parameter)
        {
            Track = parameter;
        }

        public override Task Initialize()
        {
            Sections = BuildSections();
            return base.Initialize();
        }

        public IEnumerable<ListSection<IListContentItem>> BuildSections()
        {
            var externalRelations = BuildExternalRelations().ToList();
            var aboutTrackInfos = BuildAboutTrackInfos();

            var sections = new List<ListSection<IListContentItem>>();

            if (externalRelations.Any())
            {
                sections.Add(new ListSection<IListContentItem>
                {
                    Title = TextSource.GetText("ExternalReferences"),
                    Items = externalRelations
                });
            }

            sections.Add(new ListSection<IListContentItem>
            {
                Title = TextSource.GetText("AboutTrack"),
                Items = aboutTrackInfos
            });

            return sections;
        }

        private IEnumerable<IListContentItem> BuildExternalRelations()
        {
            return GetRelationsOfType<TrackRelationExternal>()
                .Select(relation => new ExternalRelationListItem
                {
                    Title = relation.Name,
                    OnSelected = new MvxCommand(() =>
                    {
                        TryOpenExternalRelation(relation);
                    })
                });
        }

        private void TryOpenExternalRelation(TrackRelationExternal relation)
        {
            try
            {
                _uriOpener.OpenUri(new Uri(relation.Url));
            }
            catch (FormatException ex)
            {
                _exceptionHandler.HandleException(ex);
            }
        }

        private IEnumerable<IListContentItem> BuildAboutTrackInfos()
        {
            var items = new List<IListContentItem>();

            items.Add(new SelectableListItem
            {
                Title = TextSource.GetText("TrackTitle"),
                Text = Track.Meta.Title
            });

            if (SongNumbers.Any())
            {
                items.Add(new SelectableListItem
                {
                    Title = TextSource.GetText("SongNumber"),
                    Text = SeparateByComma(SongNumbers)
                });
            }

            items.Add(new SelectableListItem
            {
                Title = TextSource.GetText("Album"),
                Text = Track.Meta.Album
            });

            if (InterpreterNames.Any())
            {
                items.Add(new SelectableListItem
                {
                    Title = TextSource.GetText("Artist"),
                    Text = SeparateByComma(InterpreterNames)
                });
            }

            if (HasDuration && !Track.IsLivePlayback)
            {
                items.Add(new SelectableListItem
                {
                    Title = TextSource.GetText("Duration"),
                    Text = FormattedDuration
                });
            }

            if (Track.PublishedAt != DateTime.MinValue)
            {
                items.Add(new SelectableListItem
                {
                    Title = TextSource.GetText("PublishDate"),
                    Text = TimeInNorway
                });
            }

            if (LyricistNames.Any())
            {
                items.Add(new SelectableListItem
                {
                    Title = TextSource.GetText("Lyricist"),
                    Text = SeparateByComma(LyricistNames)
                });
            }

            if (ComposerNames.Any())
            {
                items.Add(new SelectableListItem
                {
                    Title = TextSource.GetText("Composer"),
                    Text = SeparateByComma(ComposerNames)
                });
            }

            if (ArrangerNames.Any())
			{
                items.Add(new SelectableListItem
                {
                    Title = TextSource.GetText("Arranger"),
                    Text = SeparateByComma(ArrangerNames)
                });
            }

            if (!string.IsNullOrWhiteSpace(Track?.Meta?.Publisher))
            {
                items.Add(new SelectableListItem
                {
                    Title = TextSource.GetText("Publisher"),
                    Text = Track.Meta.Publisher
                });
            }

            if (!string.IsNullOrWhiteSpace(Track?.Meta?.Copyright))
            {
                items.Add(new SelectableListItem
                {
                    Title = TextSource.GetText("Copyright"),
                    Text = Track.Meta.Copyright
                });
            }

            return items;
        }

        private string FormattedDuration
        {
            get
            {
                var duration = FirstFile.Duration;

                return $"{Math.Floor((double) duration / 60):00}:{duration % 60:00}";
            }
        }

        private bool HasDuration => FirstFile != null;

        private string TimeInNorway
        {
            get
            {
                var converter = new TrackToPublishedDateValueConverter();
                return converter.Convert(Track, null, null, System.Globalization.CultureInfo.CurrentCulture).ToString();
            }
        }

        private TrackMediaFile FirstFile
        {
            get
            {
                var firstMedia = Track?.Media?.FirstOrDefault();
                var firstFile = firstMedia?.Files.FirstOrDefault();

                return firstFile;
            }
        }

        private IEnumerable<string> SongNumbers
        {
            get
            {
                return GetRelationsOfType<TrackRelationSongbook>()
                    .Select(song => song.ShortName);
            }
        }

        private IEnumerable<string> InterpreterNames
        {
            get
            {
                return GetRelationsOfType<TrackRelationInterpreter>()
                    .Select(interpreter => interpreter.Name);
            }
        }

        private IEnumerable<string> LyricistNames
        {
            get
            {
                return GetRelationsOfType<TrackRelationLyricist>()
                    .Select(lyricist => lyricist.Name);
            }
        }

        private IEnumerable<string> ComposerNames
        {
            get
            {
                return GetRelationsOfType<TrackRelationComposer>()
                    .Select(composer => composer.Name);
            }
        }

        private IEnumerable<string> ArrangerNames
        {
            get
            {
                return GetRelationsOfType<TrackRelationArranger>()
                    .Select(arranger => arranger.Name);
            }
        }

        private IEnumerable<T> GetRelationsOfType<T>() where T: TrackRelation
        {
            return Track?.Relations?.OfType<T>() ?? Enumerable.Empty<T>();
        }

        private string SeparateByComma(IEnumerable<string> strings)
        {
            return string.Join(", ", strings);
        }
    }
}