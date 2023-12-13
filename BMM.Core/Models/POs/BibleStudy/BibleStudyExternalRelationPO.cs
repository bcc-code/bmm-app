using BMM.Core.Helpers;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy;

public class BibleStudyExternalRelationPO : BasePO, IBibleStudyExternalRelationPO
{
    private const char Separator = '/'; 
    private readonly IMediaPlayer _mediaPlayer;
    private readonly int? _trackId;
    private bool _isCurrentlyPlaying;

    public BibleStudyExternalRelationPO(
        string title,
        bool hasListened,
        Uri link,
        IDeepLinkHandler deepLinkHandler,
        IUriOpener uriOpener,
        IMediaPlayer mediaPlayer)
    {
        HasListened = hasListened;
        _mediaPlayer = mediaPlayer;

        string[] splitTitle = title.Split(Separator);
        
        if (splitTitle.Length > 1)
        {
            Title = splitTitle[0].Trim();
            Subtitle = splitTitle[1].Trim();
        }
        else
        {
            Title = title;
        }
        
        ClickedCommand = new ExceptionHandlingCommand(() =>
        {
            if (_mediaPlayer.CurrentTrack?.Id == _trackId)
            {
                _mediaPlayer.PlayPause();
                return Task.CompletedTask;
            }
            
            uriOpener.OpenUri(link);
            return Task.CompletedTask;;
        });
        
        _trackId = deepLinkHandler.GetTrackIdToPlayIfPossible(link);
        RefreshState();
    }
    
    public string Title { get; }
    public string Subtitle { get; }
    public bool HasListened { get; }
    
    public bool IsCurrentlyPlaying
    {
        get => _isCurrentlyPlaying;
        set => SetProperty(ref _isCurrentlyPlaying, value);
    }

    public bool WillPlayTrack => _trackId != null;
    public IMvxAsyncCommand ClickedCommand { get; }
    
    public Task RefreshState()
    {
        if (_trackId == null)
            return Task.CompletedTask;
        
        bool isCurrentlySelected = _mediaPlayer.CurrentTrack != null && _mediaPlayer.CurrentTrack.Id.Equals(_trackId);
        IsCurrentlyPlaying = isCurrentlySelected && _mediaPlayer.IsPlaying;
        
        return Task.CompletedTask;
    }
}