using _Microsoft.Android.Resource.Designer;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using BMM.Core.Helpers;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Translation;
using BMM.UI.Droid.Application.Adapters.Swipes;
using BMM.UI.Droid.Application.CustomViews.Swipes;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.ViewHolders.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders;

public class TrackSwipeItemViewHolder : SwipeMenuViewHolder
{
    public TrackSwipeItemViewHolder(
        View itemView,
        IMvxAndroidBindingContext context,
        ISwipeMenuAdapter swipeMenuAdapter) : base(itemView,
        context,
        swipeMenuAdapter)
    {
    }

    public TrackSwipeItemViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
    {
    }

    protected override void SetupMenuAndBind()
    {
        var set = this.CreateBindingSet<TrackSwipeItemViewHolder, TrackPO>();
        
        RightMenu.AddSwipeMenuItem(CreateAddToPlaylist(set));
        LeftMenu.AddSwipeMenuItem(CreatePlayNext(set));
        
        set.Apply();
    }

    private SwipeMenuView CreatePlayNext(MvxFluentBindingDescriptionSet<TrackSwipeItemViewHolder, TrackPO> set)
    {
        var buy = new SwipeMenuView(Context);
        
        set.Bind(buy.TitleLabel)
            .To(po => po.TextSource[Translations.UserDialogs_Track_QueueToPlayNext]);

        set.Bind(buy)
            .For(v => v.ClickCommand)
            .To(po => po.PlayNextCommand);
        
        set.Bind(buy)
            .For(v => v.FullSwipeCommand)
            .To(po => po.PlayNextCommand);
        
        buy.SetColors(Context.GetColorFromTheme(Resource.Attribute.tint_color));
        buy.TitleLabel.SetTextColor(Context.GetColorFromResource(Resource.Color.global_black_two));
        return buy;
    }

    private SwipeMenuView CreateAddToPlaylist(MvxFluentBindingDescriptionSet<TrackSwipeItemViewHolder, TrackPO> set)
    {
        var buy = new SwipeMenuView(Context);
        
        set.Bind(buy.TitleLabel)
            .To(po => po.TextSource[Translations.UserDialogs_Track_AddToPlaylist]);

        set.Bind(buy)
            .For(v => v.ClickCommand)
            .To(po => po.AddToPlaylistCommand);
        
        set.Bind(buy)
            .For(v => v.FullSwipeCommand)
            .To(po => po.AddToPlaylistCommand);
        
        buy.SetColors(Context.GetColorFromTheme(Resource.Attribute.tint_color));
        buy.TitleLabel.SetTextColor(Context.GetColorFromResource(Resource.Color.global_black_two));
        return buy;
    }
}