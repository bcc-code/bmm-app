using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.TrackInfo.Interfaces;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Models;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Other.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using MvvmCross.Commands;

namespace BMM.Core.GuardedActions.TrackInfo;

public class BuildTrackInfoSectionsAction 
    : GuardedActionWithParameterAndResult<Track, IEnumerable<IBasePO>>,
      IBuildTrackInfoSectionsAction
{
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly IUriOpener _uriOpener;
    private readonly IExceptionHandler _exceptionHandler;
    private Track _track;

    public BuildTrackInfoSectionsAction(
        IBMMLanguageBinder bmmLanguageBinder,
        IUriOpener uriOpener,
        IExceptionHandler exceptionHandler)
    {
        _bmmLanguageBinder = bmmLanguageBinder;
        _uriOpener = uriOpener;
        _exceptionHandler = exceptionHandler;
    }

    protected override Task<IEnumerable<IBasePO>> Execute(Track track)
    {
        _track = track;
        return Task.FromResult(BuildSections());
    }
    
    public IEnumerable<IBasePO> BuildSections()
    {
        var externalRelations = BuildExternalRelations().ToList();
        var aboutTrackInfos = BuildAboutTrackInfos();

        var items = new List<IBasePO>();

        if (externalRelations.Any() && !_track.IsBibleStudyProjectTrack())
        {
            items.Add(new SectionHeaderPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_ExternalReferences], false));
            items.AddRange(externalRelations);
        }

        items.Add(new SectionHeaderPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_AboutTrack], false));
        items.AddRange(aboutTrackInfos);

        return items;
    }

    private IEnumerable<ISelectableListContentItemPO> BuildExternalRelations()
    {
        return GetRelationsOfType<TrackRelationExternal>()
            .Select(relation =>
            {
                return new ExternalRelationListItemPO(
                    relation.Name,
                    string.Empty,
                    new MvxCommand(() => { TryOpenExternalRelation(relation); }));
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

    private IEnumerable<ISelectableListContentItemPO> BuildAboutTrackInfos()
    {
        var items = new List<ISelectableListContentItemPO>();

        items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_TrackTitle], _track.Meta.Title));

        if (SongNumbers.Any())
            items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_SongNumber], SeparateByComma(SongNumbers)));

        items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_Album], _track.Meta.Album));
        
        if (InterpreterNames.Any())
            items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_Artist], SeparateByComma(InterpreterNames)));

        if (HasDuration && !_track.IsLivePlayback)
            items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_Duration], FormattedDuration));

        if (_track.PublishedAt != DateTime.MinValue)
            items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_PublishDate], _track.GetPublishDate()));

        if (LyricistNames.Any())
            items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_Lyricist], SeparateByComma(LyricistNames)));

        if (ComposerNames.Any())
            items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_Composer],  SeparateByComma(ComposerNames)));

        if (ArrangerNames.Any())
            items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_Arranger],  SeparateByComma(ArrangerNames)));

        if (!string.IsNullOrWhiteSpace(_track.Meta?.Publisher))
            items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_Publisher], _track.Meta.Publisher));

        if (!string.IsNullOrWhiteSpace(_track.Meta?.Copyright))
            items.Add(new SelectableContentItemPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_Copyright], _track.Meta.Copyright));

        return items;
    }

    private string FormattedDuration
    {
        get
        {
            var duration = FirstFile.Duration;

            return $"{Math.Floor((double)duration / 60):00}:{duration % 60:00}";
        }
    }

    private bool HasDuration => FirstFile != null;

    private TrackMediaFile FirstFile
    {
        get
        {
            var firstMedia = _track.Media?.FirstOrDefault();
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

    private IEnumerable<T> GetRelationsOfType<T>()
        where T : TrackRelation
    {
        return _track.Relations?.OfType<T>();
    }

    private string SeparateByComma(IEnumerable<string> strings)
    {
        return string.Join(", ", strings);
    }
}