using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders;

public class TracklistHeaderViewHolder : MvxRecyclerViewHolder
{
    public TracklistHeaderViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
    {
        this.DelayBind(Bind);
    }

    protected TracklistHeaderViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
    {
    }

    private void Bind()
    {
        var durationSection = ItemView.FindViewById<LinearLayout>(Resource.Id.DurationSection);
        bool shouldShowDurationSection = DataContext is DownloadViewModel;
        durationSection.Visibility = shouldShowDurationSection
            ? ViewStates.Visible
            : ViewStates.Gone;
    }
}