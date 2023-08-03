using BMM.Core.Helpers;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy;

public class BibleStudyExternalRelationPO : BasePO, IBibleStudyExternalRelationPO
{
    private readonly IDeepLinkHandler _deepLinkHandler;

    public BibleStudyExternalRelationPO(
        string title,
        Uri link,
        IDeepLinkHandler deepLinkHandler,
        IUriOpener uriOpener)
    {
        Title = title;
        Link = link;
        ClickedCommand = new ExceptionHandlingCommand(() =>
        {
            uriOpener.OpenUri(Link);
            return Task.CompletedTask;
        });
        _deepLinkHandler = deepLinkHandler;
    }
    
    public string Title { get; }
    public Uri Link { get; }
    
    public bool WillPlayTrack => _deepLinkHandler.WillDeepLinkStartPlayer(Link);
    public IMvxAsyncCommand ClickedCommand { get; }
}