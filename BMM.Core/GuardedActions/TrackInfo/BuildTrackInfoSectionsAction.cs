using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.TrackInfo.Base;
using BMM.Core.GuardedActions.TrackInfo.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.Other.Interfaces;
using BMM.Core.Translation;

namespace BMM.Core.GuardedActions.TrackInfo;

public class BuildTrackInfoSectionsAction
    : BaseTrackInfoAction,
      IBuildTrackInfoSectionsAction
{
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private Track _track;

    public BuildTrackInfoSectionsAction(IBMMLanguageBinder bmmLanguageBinder)
    {
        _bmmLanguageBinder = bmmLanguageBinder;
    }

    protected override Track Track => _track;

    protected override Task<IEnumerable<IBasePO>> Execute(Track track)
    {
        _track = track;
        return Task.FromResult(BuildSections());
    }
    
    public IEnumerable<IBasePO> BuildSections()
    {
        var aboutTrackInfos = BuildAboutTrackInfos();

        var items = new List<IBasePO>
        {
            new SectionHeaderPO(_bmmLanguageBinder[Translations.TrackInfoViewModel_AboutTrack], false)
        };

        items.AddRange(aboutTrackInfos);
        return items;
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

    private string SeparateByComma(IEnumerable<string> strings)
    {
        return string.Join(", ", strings);
    }
}