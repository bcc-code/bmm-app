using System.Text;
using System.Threading.Tasks;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Localization;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using Foundation;
using Intents;
using IntentsUI;

namespace BMM.UI.iOS.Utils
{
    public static class SiriUtils
    {
        public static async Task DonatePlayMusicShortcut()
        {
            var playMusicIntent = CreatePlayMusicIntent();
            var interaction = new INInteraction(playMusicIntent, new INAddMediaIntentResponse(INAddMediaIntentResponseCode.Success, null));
            await interaction.DonateInteractionAsync();
        }
        
        public static async Task DonateFromKaareShortcut()
        {
            var fromKaareIntent = CreateFromKaareIntent();
            var interaction = new INInteraction(fromKaareIntent, new INAddMediaIntentResponse(INAddMediaIntentResponseCode.Success, null));
            await interaction.DonateInteractionAsync();
        }
        
        public static async Task AddFromKaareShortcut()
        {
            var tcs = new TaskCompletionSource<bool>();
            
            var fromKaareIntent = CreateFromKaareIntent();
            var viewController = new INUIAddVoiceShortcutViewController(new INShortcut(fromKaareIntent));
            viewController.Delegate = new AddVoiceShortcutViewControllerDelegate();
            SiriShortcutsViewController.CurrentInstance.PresentViewController(viewController, true,
                () =>
                {
                    tcs.TrySetResult(true);
                });
            
            await tcs.Task;
        }
        
        public static async Task AddPlayMusicShortcut()
        {
            var tcs = new TaskCompletionSource<bool>();
            
            var playMusicIntent = CreatePlayMusicIntent();
            var viewController = new INUIAddVoiceShortcutViewController(new INShortcut(playMusicIntent));
            viewController.Delegate = new AddVoiceShortcutViewControllerDelegate();
            SiriShortcutsViewController.CurrentInstance.PresentViewController(viewController, true,
                () =>
                {
                    tcs.TrySetResult(true);
                });
            
            await tcs.Task;
        }

        public static async Task AskForAuthorizationAndPopulateUserVocabulary()
        {
            await INPreferences.RequestSiriAuthorizationAsync();
            PopulateSiriUserDictionary();
        }

        private static void PopulateSiriUserDictionary()
        {
            NSObject.InvokeInBackground(() =>
            {
                var vocabulary = new NSMutableOrderedSet<NSString>();
                
                foreach (string fromKareSiriPhrase in SiriConstants.FromKareSiriPhrases)
                    vocabulary.Add(new NSString(fromKareSiriPhrase));
                
                vocabulary.Add(new NSString(BMMLanguageBinderLocator.TextSource[Translations.ExploreNewestViewModel_FraKaareHeader]));
                
                INVocabulary.SharedVocabulary.RemoveAllVocabularyStrings();
                INVocabulary.SharedVocabulary.SetVocabularyStrings(new NSOrderedSet<NSString>(vocabulary), INVocabularyStringType.MediaShowTitle);
            });
        }

        private static INPlayMediaIntent CreateFromKaareIntent()
        {
            var fromKaareMediaItem = new INMediaItem(
                SiriConstants.FromKareIdentifier,
                BMMLanguageBinderLocator.TextSource[Translations.ExploreNewestViewModel_FraKaareHeader],
                INMediaItemType.PodcastEpisode,
                null);

            var intent = new INPlayMediaIntent(
                fromKaareMediaItem.EncloseInArray(),
                null,
                null,
                INPlaybackRepeatMode.None,
                null,
                INPlaybackQueueLocation.Unknown,
                null,
                null);
            
            intent.SuggestedInvocationPhrase = BMMLanguageBinderLocator.TextSource[Translations.Global_SiriFromKaareInvocationPhrase];
            return intent;
        }
        
        private static INPlayMediaIntent CreatePlayMusicIntent()
        {
            var playMusicMediaItem = new INMediaItem(
                SiriConstants.PlayMusicIdentifier,
                BMMLanguageBinderLocator.TextSource[Translations.Browse_MusicTitle],
                INMediaItemType.Music,
                null);
            
            var intent = new INPlayMediaIntent(
                playMusicMediaItem.EncloseInArray(),
                null,
                null,
                INPlaybackRepeatMode.None,
                null,
                INPlaybackQueueLocation.Unknown,
                null,
                null);

            intent.SuggestedInvocationPhrase = BMMLanguageBinderLocator.TextSource[Translations.Global_SiriPlayMusicInvocationPhrase];
            return intent;
        }

        public static string CreatePlaybackOrigin(SiriSource siriSource, string phrase = "")
        {
            var playbackOriginBuilder =  new StringBuilder($"{SiriConstants.PlaybackOrigin}|{siriSource}");

            if (!string.IsNullOrEmpty(phrase))
                playbackOriginBuilder.Append($"|{phrase}");

            return playbackOriginBuilder.ToString();
        }
    }

    public enum SiriSource
    {
        PlayMusic,
        FraKaare,
        Search
    }
}