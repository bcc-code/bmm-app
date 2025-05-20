using System.Reflection;
using Acr.UserDialogs;
using BMM.Core.Helpers;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer.Abstractions;
using CoreFoundation;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding;
using MvvmCross.Core;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.ViewModels;

namespace BMM.UI.iOS;

[Register(nameof(BmmWindowSceneDelegate))]
public class BmmWindowSceneDelegate : UIWindowSceneDelegate
{
    private bool _shouldPreventQueueRecovery;
    public AppDelegate AppDelegateInstance => (AppDelegate)UIApplication.SharedApplication.Delegate;
    public UIWindow Window { get; set; }

    public override void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        if (scene is not UIWindowScene windowScene)
            return;
        
        Window = new UIWindow(windowScene);
        CheckShouldPreventQueueRecovery();
        
        MvxSingleton.ClearAllSingletons();
        typeof(MvxIoCProvider)
            .GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)
            ?.SetValue(null, null);
        
        MvxIosSetupSingleton.EnsureSingletonAvailable(AppDelegateInstance, Window).EnsureInitialized();
        AppDelegate.MainWindow = Window;
        AppDelegateInstance.Window = Window;
        AppDelegateInstance.RegisterForNotifications();
        AppDelegateInstance.SetThemeForApp();
        
        PreventQueueRecoveringIfNeeded();
        RunAppStart();

        if (connectionOptions.UserActivities == null)
            return;
        
        foreach (var urlContext in connectionOptions.UserActivities)
            HandleDeepLink(urlContext.WebPageUrl?.AbsoluteString);
    }
    
    public override void ContinueUserActivity(UIScene scene, NSUserActivity userActivity)
    {
        HandleDeepLink(userActivity?.WebPageUrl?.AbsoluteString);
    }

    private void HandleDeepLink(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;

        var deepLinkHandler = Mvx.IoCProvider!.Resolve<IDeepLinkHandler>();
        deepLinkHandler!.SetDeepLinkWillStartPlayerIfNeeded(url);
        deepLinkHandler.OpenFromOutsideOfApp(new Uri(url));
    }
    
    private void CheckShouldPreventQueueRecovery()
    {
        if (Mvx.IoCProvider == null || !Mvx.IoCProvider.TryResolve<IMediaPlayer>(out var mediaPlayer))
            return;

        _shouldPreventQueueRecovery = mediaPlayer!.IsPlaying;
    }

    /// <summary>
    ///     We need to prevent queue recovery when the app is playing track from CarPlay
    ///     and then opened on mobile phone. It is necessary to smooth share player state from CarPlay to Mobile.
    /// </summary>
    private void PreventQueueRecoveringIfNeeded()
    {
        if (!_shouldPreventQueueRecovery)
            return;
        
        var rememberedQueueService = Mvx.IoCProvider!.Resolve<IRememberedQueueService>();
        rememberedQueueService!.SetPlayerHasPendingOperation();
    }

    public override void WillEnterForeground(UIScene scene)
    {
        AppDelegateInstance.FireLifetimeChanged(MvxLifetimeEvent.ActivatedFromMemory);
    }

    public override void DidEnterBackground(UIScene scene)
    {
        AppDelegateInstance.FireLifetimeChanged(MvxLifetimeEvent.Deactivated);
    }

    public override void DidDisconnect(UIScene scene)
    {
        AppDelegateInstance.FireLifetimeChanged(MvxLifetimeEvent.Closing);
    }

    protected virtual void RunAppStart()
    {
        if (Mvx.IoCProvider?.TryResolve(out IMvxAppStart startup) == true && !startup.IsStarted)
            startup.Start();
        
        Window!.MakeKeyAndVisible();
    }
}