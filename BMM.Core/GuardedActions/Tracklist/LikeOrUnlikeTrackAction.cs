using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Tracklist.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Messages;
using BMM.Core.Models.TrackCollections.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.GuardedActions.Tracklist;

public class LikeOrUnlikeTrackAction 
    : GuardedActionWithParameterAndResult<ILikeOrUnlikeTrackActionParameter, bool>,
      ILikeUnlikeTrackAction
{
    private readonly ITrackCollectionClient _trackCollectionClient;
    private readonly IPlaybackHistoryService _playbackHistoryService;
    private readonly IRememberedQueueService _rememberedQueueService;
    private readonly IMvxMessenger _messenger;
    private readonly IMediaPlayer _mediaPlayer;

    public LikeOrUnlikeTrackAction(
        ITrackCollectionClient trackCollectionClient,
        IPlaybackHistoryService playbackHistoryService,
        IRememberedQueueService rememberedQueueService,
        IMvxMessenger messenger,
        IMediaPlayer mediaPlayer)
    {
        _trackCollectionClient = trackCollectionClient;
        _playbackHistoryService = playbackHistoryService;
        _rememberedQueueService = rememberedQueueService;
        _messenger = messenger;
        _mediaPlayer = mediaPlayer;
    }

    protected override async Task<bool> Execute(ILikeOrUnlikeTrackActionParameter parameter)
    {
        bool result;
        
        if (parameter.IsLiked)
            result = await Unlike(parameter.TrackId.EncloseInArray());
        else
            result = await Like(parameter.TrackId.EncloseInArray());

        if (!result)
            return false;
        
        await _playbackHistoryService.SetTrackLikedOrUnliked(parameter.TrackId, !parameter.IsLiked);
        await _rememberedQueueService.SetTrackLikedOrUnliked(parameter.TrackId, !parameter.IsLiked);
        _messenger.Publish(new TrackLikedChangedMessage(this, !parameter.IsLiked, parameter.TrackId));
        
        if (_mediaPlayer.CurrentTrack?.Id == parameter.TrackId)
            _mediaPlayer.CurrentTrack.IsLiked = !parameter.IsLiked;
        
        return true;
    }

    private async Task<bool> Unlike(IList<int> trackIds)
    {
        try
        {
            return await _trackCollectionClient.Unlike(trackIds);
        }
        catch (Exception e)
        {
            if (e is not TrackNotInTrackCollectionException)
                throw;

            return true;
        }
    }
    
    private async Task<bool> Like(IList<int> trackIds)
    {
        try
        {
            return await _trackCollectionClient.Like(trackIds);
        }
        catch (Exception e)
        {
            if (e is not TrackAlreadyInTrackCollectionException)
                throw;

            return true;
        }
    }
}