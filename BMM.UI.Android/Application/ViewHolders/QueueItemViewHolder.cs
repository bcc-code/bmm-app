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

public class QueueItemViewHolder : SwipeMenuViewHolder
{
    public QueueItemViewHolder(
        View itemView,
        IMvxAndroidBindingContext context,
        ISwipeMenuAdapter swipeMenuAdapter) : base(itemView,
        context,
        swipeMenuAdapter)
    {
    }

    public QueueItemViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
    {
    }

    protected override void SetupMenuAndBind()
    {
        var set = this.CreateBindingSet<QueueItemViewHolder, TrackPO>();
        
        RightMenu.AddSwipeMenuItem(Create(set));
        LeftMenu.AddSwipeMenuItem(Create(set));
        
        set.Apply();
    }

    private SwipeMenuView Create(MvxFluentBindingDescriptionSet<QueueItemViewHolder, TrackPO> set)
    {
        var buy = new SwipeMenuView(Context);
        
        set.Bind(buy.TitleLabel)
            .To(po => po.TextSource[Translations.QueueViewModel_Delete]);

        set.Bind(buy)
            .For(v => v.ClickCommand)
            .To(po => po.DeleteFromQueueCommand);
        
        set.Bind(buy)
            .For(v => v.FullSwipeCommand)
            .To(po => po.DeleteFromQueueCommand);
        
        buy.SetColors(Context.GetColorFromResource(ResourceConstant.Color.radio_color));
        return buy;
    }
}