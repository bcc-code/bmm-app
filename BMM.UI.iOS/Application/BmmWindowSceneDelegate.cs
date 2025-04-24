using System.Reflection;
using BMM.Core.Implementations;
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
public class BmmWindowSceneDelegate : UIResponder, IUIWindowSceneDelegate
{
    public AppDelegate AppDelegateInstance => (AppDelegate)UIApplication.SharedApplication.Delegate;
    public UIWindow Window { get; set; }

    [Export("scene:willConnectToSession:options:")]
    public void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        if (scene is not UIWindowScene windowScene)
            return;
        
        Window = new UIWindow(windowScene);
        
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