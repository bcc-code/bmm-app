using Acr.UserDialogs;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;

namespace BMM.Core.Extensions;

public static class ActionSheetConfigExtensions
{
    private static IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;
    
    public static ActionSheetConfig AddOptionForAddToTrackCollection(this ActionSheetConfig actionSheetConfig, Func<Task> action)
    {
        return actionSheetConfig.AddHandled(TextSource[Translations.UserDialogs_AddAllToPlaylist],
            async () => await action(),
            ImageResourceNames.IconFavorites);
    }
}