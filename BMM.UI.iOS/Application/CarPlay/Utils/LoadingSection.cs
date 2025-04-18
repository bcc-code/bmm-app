using System.Diagnostics.CodeAnalysis;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using CarPlay;
using MvvmCross;

namespace BMM.UI.iOS.CarPlay.Utils;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
public static class LoadingSection
{
    private static IBMMLanguageBinder LanguageBinder => Mvx.IoCProvider!.Resolve<IBMMLanguageBinder>();
    
    public static CPListSection[] Create()
    {
        var loadingItem = new CPListItem(LanguageBinder[Translations.CarPlay_LoadingText], null);
        loadingItem.AccessoryType = CPListItemAccessoryType.None;
        loadingItem.Handler = (_, block) => block();
        return
        [
            new CPListSection([(ICPListTemplateItem)loadingItem])
        ];
    }
}