using System.Diagnostics.CodeAnalysis;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using BMM.UI.iOS.CarPlay.Creators.Base;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.CarPlay.Utils;
using BMM.UI.iOS.Extensions;
using CarPlay;
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class PlaylistsLayoutCreator : BaseLayoutCreator, IPlaylistsLayoutCreator
{
    private IPlaylistClient PlaylistClient => Mvx.IoCProvider!.Resolve<IPlaylistClient>();
    private IBMMLanguageBinder BMMLanguageBinder => Mvx.IoCProvider!.Resolve<IBMMLanguageBinder>();
    private IPlaylistLayoutCreator PlaylistLayoutCreator => Mvx.IoCProvider!.Resolve<IPlaylistLayoutCreator>();

    private CPListTemplate _playlistsListTemplate;
    private CPInterfaceController _cpInterfaceController;

    protected override CPInterfaceController CpInterfaceController => _cpInterfaceController;

    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        _cpInterfaceController = cpInterfaceController;
        _playlistsListTemplate = new CPListTemplate(BMMLanguageBinder[Translations.CuratedPlaylistsViewModel_Title], LoadingSection.Create());
        _playlistsListTemplate.TabTitle = BMMLanguageBinder[Translations.CuratedPlaylistsViewModel_Title];
        _playlistsListTemplate.TabImage = UIImage.FromBundle(ImageResourceNames.IconPlaylist.ToIosImageName());
        Load().FireAndForget();
        return _playlistsListTemplate;
    }

    public override async Task Load()
    {
        var playlists = await PlaylistClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);
        
        var playlistListItemTemplates = await Task.WhenAll(playlists
            .Select(async playlist =>
            {
                var coverImage = await playlist.Cover.ToUIImage();
                var trackListItem = new CPListItem(playlist.Title, null, coverImage);

                trackListItem.Handler = async (item, block) =>
                {
                    var playlistLayout = await PlaylistLayoutCreator.Create(CpInterfaceController, playlist.Id, playlist.Title);
                    await CpInterfaceController.PushTemplateAsync(playlistLayout, true);
                    block();
                };

                trackListItem.AccessoryType = CPListItemAccessoryType.None;
                return (ICPListTemplateItem)trackListItem;
            }));
        
        var section = new CPListSection(playlistListItemTemplates.ToArray());
        _playlistsListTemplate.UpdateSections(section.EncloseInArray());
    }
}