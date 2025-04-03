using System.Diagnostics.CodeAnalysis;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using BMM.UI.iOS.CarPlay.Creators.Interfaces;
using BMM.UI.iOS.Extensions;
using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public class FavouritesLayoutCreator : IFavouritesLayoutCreator
{
    private readonly IBMMLanguageBinder _bmmLanguageBinder;

    public FavouritesLayoutCreator(IBMMLanguageBinder bmmLanguageBinder)
    {
        _bmmLanguageBinder = bmmLanguageBinder;
    }
    
    public async Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController)
    {
        await Task.CompletedTask;
        var favouritesListTemplate = new CPListTemplate(_bmmLanguageBinder[Translations.MenuViewModel_Favorites], []);
        favouritesListTemplate.TabTitle = _bmmLanguageBinder[Translations.MenuViewModel_Favorites];
        favouritesListTemplate.TabImage = UIImage.FromBundle("icon_favorites".ToNameWithExtension());
        return favouritesListTemplate;
    }
}