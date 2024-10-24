using BMM.Core.Helpers;
using BMM.Core.Implementations.DeepLinking;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.BibleStudy;

public class BibleStudyExternalRelationPO : BasePO, IBibleStudyExternalRelationPO
{
    private const char Separator = '/'; 
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IMvxNavigationService _mvxNavigationService;
    private readonly int? _trackId;
    private readonly int? _questionId;
    private bool _isCurrentlyPlaying;

    public BibleStudyExternalRelationPO(
        string title,
        bool hasListened,
        Uri link,
        IDeepLinkHandler deepLinkHandler,
        IUriOpener uriOpener,
        IMediaPlayer mediaPlayer,
        IMvxNavigationService mvxNavigationService)
    {
        HasListened = hasListened;
        _mediaPlayer = mediaPlayer;
        _mvxNavigationService = mvxNavigationService;

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
        
        ClickedCommand = new ExceptionHandlingCommand(async () =>
        {
            if (_questionId != null)
            {
                await _mvxNavigationService.Navigate<QuizQuestionViewModel, IQuizQuestionViewModelParameter>(
                    new QuizQuestionViewModelParameter(_questionId.Value));
                return;
            }
            
            if (_trackId != null && _mediaPlayer.CurrentTrack?.Id == _trackId)
            {
                _mediaPlayer.PlayPause();
                return;
            }

            uriOpener.OpenUri(link);
        });
        
        _trackId = deepLinkHandler.GetIdFromUriIfPossible(link, DeepLinkHandler.PlayTrackRegex);
        _questionId = deepLinkHandler.GetIdFromUriIfPossible(link, DeepLinkHandler.QuizRegex);
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
    public bool HasQuestion => _questionId != null;
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