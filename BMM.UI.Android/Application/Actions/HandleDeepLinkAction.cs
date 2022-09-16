using System;
using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Helpers;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Actions.Interfaces;
using BMM.UI.Droid.Application.Activities;
using BMM.UI.Droid.Application.NewMediaPlayer.Controller;

namespace BMM.UI.Droid.Application.Actions
{
    public class HandleDeepLinkAction
        : GuardedActionWithParameter<string>,
          IHandleDeepLinkAction
    {
        private readonly IDeepLinkHandler _deepLinkHandler;
        private readonly AndroidMediaPlayer _mediaPlayer;

        public HandleDeepLinkAction(
            IDeepLinkHandler deepLinkHandler,
            IPlatformSpecificMediaPlayer mediaPlayer)
        {
            _deepLinkHandler = deepLinkHandler;
            _mediaPlayer = (AndroidMediaPlayer)mediaPlayer;
        }
        
        protected override async Task Execute(string deepLink)
        {
            await Task.CompletedTask;

            if (string.IsNullOrEmpty(deepLink))
                return;

            if (_mediaPlayer.IsConnected)
            {
                _deepLinkHandler.OpenFromOutsideOfApp(new Uri(deepLink));
                MainActivity.UnhandledDeepLink = null;
                return;
            }
            
            MainActivity.UnhandledDeepLink = deepLink;
            _deepLinkHandler.SetDeepLinkWillStartPlayerIfNeeded(deepLink);
        }
    }
}