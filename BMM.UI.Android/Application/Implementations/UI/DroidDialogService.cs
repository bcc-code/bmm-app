using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.Parameters;
using BMM.UI.Droid.Application.CustomViews;
using JetBrains.Annotations;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Implementations.UI;

public class DroidDialogService : IDialogService
{
    private readonly IMvxAndroidCurrentTopActivity _androidCurrentTopActivity;

    public DroidDialogService(IMvxAndroidCurrentTopActivity androidCurrentTopActivity)
    {
        _androidCurrentTopActivity = androidCurrentTopActivity;
    }
    
    public Task ShowDialog(IDialogParameter dialog)
    {
        var currentActivity = _androidCurrentTopActivity.Activity;
        var dialogView = new BmmDialog(currentActivity);
        var alert = new Dialog(currentActivity);
        dialog.CloseAction += () =>
        {
            alert.Dismiss();
        };
        dialogView.BindingContext.DataContext = dialog;
        alert!.Window!.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
        alert.SetContentView(dialogView);
        alert.SetCancelable(false);
        
        currentActivity.RunOnUiThread(
            () =>
            {
                alert.Show();
            });
        
        return Task.CompletedTask;
    }
}