using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.MyContent.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.MyContent;

public class PrepareMyContentItemsAction
    : GuardedActionWithParameterAndResult<IEnumerable<IDocumentPO>, IList<IDocumentPO>>,
      IPrepareMyContentItemsAction
{
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly IMvxNavigationService _mvxNavigationService;

    public PrepareMyContentItemsAction(
        IBMMLanguageBinder bmmLanguageBinder,
        IMvxNavigationService mvxNavigationService)
    {
        _bmmLanguageBinder = bmmLanguageBinder;
        _mvxNavigationService = mvxNavigationService;
    }
    
    protected override async Task<IList<IDocumentPO>> Execute(IEnumerable<IDocumentPO> documentsCollections)
    {
        await Task.CompletedTask;
        var listOfDocuments = new List<IDocumentPO>();

        var trackCollectionsList = documentsCollections
            .OfType<TrackCollectionPO>()
            .ToList();

        var sharedWithMe = trackCollectionsList
            .Where(t => !t.TrackCollection.CanEdit)
            .ToList();

        var trackCollectionList = trackCollectionsList
            .Except(sharedWithMe)
            .ToList();

        var myPlaylistHeader = GetHeader(Translations.MyContentViewModel_MyPlaylists);

        listOfDocuments.Add(new ChapterHeaderPO(myPlaylistHeader));
        listOfDocuments.AddRange(BuildPinnedItems());
        listOfDocuments.AddRange(trackCollectionList);

        if (!sharedWithMe.Any())
            return listOfDocuments;

        var sharedWithMeHeader = GetHeader(Translations.MyContentViewModel_SharedWithMe);

        listOfDocuments.Add(new ChapterHeaderPO(sharedWithMeHeader));
        listOfDocuments.AddRange(sharedWithMe);

        return listOfDocuments;
    }
    
    private ChapterHeader GetHeader(string titleKey)
    {
        return new ChapterHeader
        {
            DocumentType = DocumentType.ChapterHeader,
            Title = _bmmLanguageBinder[titleKey]
        };
    }

    private IEnumerable<PinnedItemPO> BuildPinnedItems()
    {
        return new List<PinnedItem>
        {
            new()
            {
                Title = _bmmLanguageBinder[Translations.MyContentViewModel_DownloadedContent],
                Action = new MvxAsyncCommand<PinnedItem>(async execute => await _mvxNavigationService.Navigate<DownloadedContentViewModel>()),
                Icon = "icon_download",
                ActionType = PinnedItemActionType.DownloadedContent
            },
            new()
            {
                Title = _bmmLanguageBinder[Translations.MyContentViewModel_FollowedPodcasts],
                Action = new MvxAsyncCommand<PinnedItem>(async execute => await _mvxNavigationService.Navigate<FollowedPodcastsViewModel>()),
                Icon = "icon_podcast",
                ActionType = PinnedItemActionType.FollowedPodcasts
            }
        }.Select(pi => new PinnedItemPO(pi));
    }
}