using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Contributors.Interfaces;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.Contributors;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.UI.iOS.Extensions;
using CarPlay;
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Utils;

public class ShuffleButtonCreator
{
    private static IBMMLanguageBinder TextSource => Mvx.IoCProvider!.Resolve<IBMMLanguageBinder>();
    private static IMediaPlayer MediaPlayer => Mvx.IoCProvider!.Resolve<IMediaPlayer>();
    private static IShuffleContributorAction ShuffleContributorAction => Mvx.IoCProvider!.Resolve<IShuffleContributorAction>();
    private static IShufflePodcastAction ShufflePodcastAction => Mvx.IoCProvider!.Resolve<IShufflePodcastAction>();
    
    public static ICPListTemplateItem Create(
        IEnumerable<Document> items,
        string playbackOrigin,
        CPInterfaceController cpInterfaceController)
    {
        if (!items.All(c => c is Track))
            return null;
        
        var listItem = new CPListItem(TextSource[Translations.DocumentsViewModel_Shuffle],
            null,
            UIImage.FromBundle(ImageResourceNames.IconDownloadedCarplay.ToIosImageName()));

        listItem.Handler = async (_, block) =>
        {
            await MediaPlayer.ShuffleList(items.OfType<IMediaTrack>().ToList(), playbackOrigin);
            await CarPlayPlayerPresenter.ShowPlayer(MediaPlayer.CurrentTrack as IMediaTrack, cpInterfaceController);
            block();
        };
        
        return listItem;
    }
    
    public static ICPListTemplateItem CreateForPodcast(
        int podcastId,
        string playbackOrigin,
        CPInterfaceController cpInterfaceController)
    {
        var listItem = new CPListItem(TextSource[Translations.DocumentsViewModel_Shuffle],
            null,
            UIImage.FromBundle(ImageResourceNames.IconDownloadedCarplay.ToIosImageName()));

        listItem.Handler = async (_, block) =>
        {
            var trackToPlay = await ShufflePodcastAction.ExecuteGuarded(new ShuffleActionParameter(podcastId, playbackOrigin));
            await CarPlayPlayerPresenter.ShowPlayer(trackToPlay, cpInterfaceController);
            block();
        };

        return listItem;
    }
    
    public static ICPListTemplateItem CreateForContributor(
        int contributorId,
        string playbackOrigin,
        CPInterfaceController cpInterfaceController)
    {
        var listItem = new CPListItem(TextSource[Translations.DocumentsViewModel_Shuffle],
            null,
            UIImage.FromBundle(ImageResourceNames.IconDownloadedCarplay.ToIosImageName()));

        listItem.Handler = async (_, block) =>
        {
            var trackToPlay = await ShuffleContributorAction.ExecuteGuarded(new ShuffleActionParameter(contributorId, playbackOrigin));
            await CarPlayPlayerPresenter.ShowPlayer(trackToPlay, cpInterfaceController);
            block();
        };

        return listItem;
    }
}