using Android.Views;
using BMM.Core.Models.POs.Tracks;
using BMM.UI.Droid.Application.CustomViews;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders;

public class HighlightedTextTrackViewHolder : MvxRecyclerViewHolder
{
    public HighlightedTextTrackViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
    {
        this.DelayBind(Bind);
    }
    
    public float RatioOfFirstHighlightPositionToFullText { get; set; }
    public float RatioOfFirstHighlightLengthToFullText { get; set; }

    private void Bind()
    {
        var set = this.CreateBindingSet<HighlightedTextTrackViewHolder, HighlightedTextTrackPO>();
        
        set.Bind(this)
            .For(v => v.RatioOfFirstHighlightPositionToFullText)
            .To(po => po.RatioOfFirstHighlightPositionToFullText);
                
        set.Bind(this)
            .For(v => v.RatioOfFirstHighlightLengthToFullText)
            .To(po => po.RatioOfFirstHighlightLengthToFullText);
                
        set.Apply();
    }

    public override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        ItemView.Post(SetContentOffset);
    }

    private void SetContentOffset()
    {
        var scrollView = ItemView.FindViewById<NonTouchableHorizontalScrollView>(Resource.Id.HighlightLabelScrollView);
        int scrollContentWidth = scrollView!.GetChildAt(0)!.MeasuredWidth;

        if (scrollContentWidth == 0)
            return;

        int scrollFrameWidth = scrollView.MeasuredWidth;
        bool contentFitsInScrollView = scrollContentWidth <= scrollFrameWidth;

        if (contentFitsInScrollView)
            return;

        var maxOffsetToEnd = scrollContentWidth - scrollFrameWidth;
        float highlightWidth = RatioOfFirstHighlightLengthToFullText * scrollContentWidth;
        float desiredCenterOffset = scrollContentWidth * RatioOfFirstHighlightPositionToFullText - scrollFrameWidth / 2 + highlightWidth;
        desiredCenterOffset = Math.Min(desiredCenterOffset, maxOffsetToEnd);
        scrollView.ScrollTo((int)Math.Max(0, desiredCenterOffset), 0);
    }
}