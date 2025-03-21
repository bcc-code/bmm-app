using MvvmCross;
using MvvmCross.Core;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.ViewModels;

namespace BMM.UI.iOS;

public abstract class BmmApplicationDelegate : UIApplicationDelegate, IMvxApplicationDelegate
{
    public new virtual UIWindow Window { get; set; }

    public BmmApplicationDelegate() : base()
    {
        RegisterSetup();
    }

    public override void WillEnterForeground(UIApplication application)
    {
        FireLifetimeChanged(MvxLifetimeEvent.ActivatedFromMemory);
    }

    public override void DidEnterBackground(UIApplication application)
    {
        FireLifetimeChanged(MvxLifetimeEvent.Deactivated);
    }

    public override void WillTerminate(UIApplication application)
    {
        FireLifetimeChanged(MvxLifetimeEvent.Closing);
    }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        FireLifetimeChanged(MvxLifetimeEvent.Launching);
        return true;
    }

    protected virtual void RegisterSetup()
    {
    }

    public void FireLifetimeChanged(MvxLifetimeEvent which)
    {
        var handler = LifetimeChanged;
        handler?.Invoke(this, new MvxLifetimeEventArgs(which));
    }

    public event EventHandler<MvxLifetimeEventArgs> LifetimeChanged;
}

public abstract class BmmApplicationDelegate<TMvxIosSetup, TApplication> : BmmApplicationDelegate
    where TMvxIosSetup : MvxIosSetup<TApplication>, new()
    where TApplication : class, IMvxApplication, new()
{
    protected override void RegisterSetup()
    {
        this.RegisterSetupType<TMvxIosSetup>();
    }
}