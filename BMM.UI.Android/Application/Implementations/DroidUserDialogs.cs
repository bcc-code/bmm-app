using System;
using Acr.UserDialogs;
using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using BMM.UI.Droid.Application.Activities;
using Google.Android.Material.Snackbar;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace BMM.UI.Droid.Application.Implementations
{
    public class DroidUserDialogs : UserDialogsImpl
    {
        public DroidUserDialogs(Func<Activity> getTopActivity) : base(getTopActivity)
        {
        }

        protected override IDisposable Show(Activity activity, Func<Dialog> dialogBuilder)
        {
            if (activity == null)
                return new DisposableAction(() => { });
            
            Dialog dialog = null;
            activity.SafeRunOnUi(() =>
            {
                dialog = dialogBuilder();
                dialog.Show();
            });
            return new DisposableAction(() =>
                activity.SafeRunOnUi(dialog.Dismiss)
            );
        }
        
        protected override IDisposable ShowDialog<TFragment, TConfig>(AppCompatActivity activity, TConfig config)
        {
            if (activity == null)
                return new DisposableAction(() => { });
            
            TFragment frag = null;
            activity.SafeRunOnUi(() =>
            {
                frag = (TFragment)Activator.CreateInstance(typeof(TFragment));
                frag.Config = config;
                frag.Show(activity.SupportFragmentManager, FragmentTag);
            });
            return new DisposableAction(() =>
                activity.SafeRunOnUi(frag.Dismiss)
            );
        }
        
        protected override IDisposable ToastAppCompat(AppCompatActivity activity, ToastConfig cfg)
        {
            Snackbar snackBar = null;
            activity.SafeRunOnUi(() =>
            {
                View view = null;
                
                var dialog = activity.SupportFragmentManager.Fragments;
                var lastFragment = dialog.Last();
                
                if (lastFragment is MvxDialogFragment dialogFragment)
                    view = dialogFragment.View;
                else
                    view = activity.Window.DecorView.RootView.FindViewById(Android.Resource.Id.Content);
                    
                var msg = GetSnackbarText(cfg);

                snackBar = Snackbar.Make(
                    view,
                    msg,
                    (int)cfg.Duration.TotalMilliseconds
                );

                if (cfg.BackgroundColor != null)
                    snackBar.View.SetBackgroundColor(cfg.BackgroundColor.Value.ToNative());

                if (cfg.Position == ToastPosition.Top)
                {
                    if (snackBar.View.LayoutParameters is FrameLayout.LayoutParams layoutParams)
                    {
                        layoutParams.Gravity = GravityFlags.Top;
                        layoutParams.SetMargins(0, 80, 0, 0);
                        snackBar.View.LayoutParameters = layoutParams;
                    }
                }
                if (cfg.Action != null)
                {
                    snackBar.SetAction(cfg.Action.Text, x =>
                    {
                        cfg.Action?.Action?.Invoke();
                        snackBar.Dismiss();
                    });
                    var color = cfg.Action.TextColor;
                    if (color != null)
                        snackBar.SetActionTextColor(color.Value.ToNative());
                }

                if ((cfg.Position == null || cfg.Position == ToastPosition.Bottom) && activity is MainActivity mainActivity)
                    snackBar.View.TranslationY = -mainActivity.GetSnackbarBottomMargin();

                snackBar.Show();
            });
            return new DisposableAction(() =>
            {
                if (snackBar.IsShown)
                    activity.SafeRunOnUi(snackBar.Dismiss);
            });
        }
    }
}