using System.Reflection;
using Acr.UserDialogs;
using BMM.Core.Helpers;
using BMM.Core.Implementations;
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
    public AppDelegate AppDelegateInstance => (AppDelegate)UIApplication.SharedApplication.Delegate;
    public UIWindow Window { get; set; }

    public override void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        if (scene is not UIWindowScene windowScene)
            return;
        
        Window = new UIWindow(windowScene);

        HandlePlayerStopIfAlreadyPlaying();
        
        MvxSingleton.ClearAllSingletons();
        typeof(MvxIoCProvider)
            .GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)
            ?.SetValue(null, null);
        
        MvxIosSetupSingleton.EnsureSingletonAvailable(AppDelegateInstance, Window).EnsureInitialized();
        AppDelegate.MainWindow = Window;
        AppDelegateInstance.Window = Window;
        AppDelegateInstance.RegisterForNotifications();
        AppDelegateInstance.SetThemeForApp();
        
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
    
    /// <summary>
    ///     We need to stop playing when starting the app to avoid two or more playbacks at the same time.
    ///     It only occurs when the app is opened as CarPlay first and then opened on phone.
    ///     Detailed investigation is needed to handle it without playing interrupting.
    /// </summary>
    private void HandlePlayerStopIfAlreadyPlaying()
    {
        if (Mvx.IoCProvider == null || !Mvx.IoCProvider.TryResolve<IMediaPlayer>(out var mediaPlayer))
            return;

        if (!mediaPlayer!.IsPlaying)
            return;
        
        mediaPlayer.Stop();
    }

    public void WillEnterForeground(UIScene scene)
    {
        AppDelegateInstance.FireLifetimeChanged(MvxLifetimeEvent.ActivatedFromMemory);
    }

    public void DidEnterBackground(UIScene scene)
    {
        AppDelegateInstance.FireLifetimeChanged(MvxLifetimeEvent.Deactivated);
    }

    public void DidDisconnect(UIScene scene)
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