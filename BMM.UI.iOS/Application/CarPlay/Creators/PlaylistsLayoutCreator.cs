using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.Extensions;
using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class PlaylistsLayoutCreator : IPlaylistsLayoutCreator
{
    private readonly IPlaylistClient _playlistClient;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly IPlaylistLayoutCreator _playlistLayoutCreator;

    public PlaylistsLayoutCreator(
        IPlaylistClient playlistClient,
        IBMMLanguageBinder bmmLanguageBinder,
        IPlaylistLayoutCreator playlistLayoutCreator)
    {
        _playlistClient = playlistClient;
        _bmmLanguageBinder = bmmLanguageBinder;
        _playlistLayoutCreator = playlistLayoutCreator;
    }
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        var playlists = await _playlistClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);
        
        var playlistListItemTemplates = await Task.WhenAll(playlists
            .Select(async playlist =>
            {
                var coverImage = await playlist.Cover.ToUIImage();
                var trackListItem = new CPListItem(playlist.Title, null, coverImage);

                trackListItem.Handler = async (item, block) =>
                {
                    var playlistLayout = await _playlistLayoutCreator.Create(cpInterfaceController, playlist.Id, playlist.Title);
                    await cpInterfaceController.PushTemplateAsync(playlistLayout, true);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                return (ICPListTemplateItem)trackListItem;
            }));
        
        var section = new CPListSection(playlistListItemTemplates.ToArray());
        var playlistsListTemplate = new CPListTemplate(_bmmLanguageBinder[Translations.CuratedPlaylistsViewModel_Title], section.EncloseInArray());
        playlistsListTemplate.TabTitle = _bmmLanguageBinder[Translations.CuratedPlaylistsViewModel_Title];
        playlistsListTemplate.TabImage = UIImage.FromBundle(ImageResourceNames.IconPlaylist.ToIosImageName());
        return playlistsListTemplate;
    }
}