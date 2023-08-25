using BMM.Core.Helpers;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy;

public class BibleStudyExternalRelationPO : BasePO, IBibleStudyExternalRelationPO
{
    private const char Separator = '/'; 
    private readonly IDeepLinkHandler _deepLinkHandler;
    private bool _showPlayAnimation;

    public BibleStudyExternalRelationPO(
        string title,
        Uri link,
        IDeepLinkHandler deepLinkHandler,
        IUriOpener uriOpener)
    {
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
        
        Link = link;
        ClickedCommand = new ExceptionHandlingCommand(() =>
        {
            uriOpener.OpenUri(Link);
            ShouldShowPlayAnimation = true;
            return Task.CompletedTask;
        });
        _deepLinkHandler = deepLinkHandler;
    }
    
    public string Title { get; }
    public string Subtitle { get; }
    public Uri Link { get; }

    public bool ShouldShowPlayAnimation
    {
        get => _showPlayAnimation;
        set => SetProperty(ref _showPlayAnimation, value);
    }

    public bool WillPlayTrack => _deepLinkHandler.WillDeepLinkStartPlayer(Link);
    public IMvxAsyncCommand ClickedCommand { get; }
}