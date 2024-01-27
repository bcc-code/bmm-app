using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Models.POs.BibleStudy;
using MvvmCross.DroidX.RecyclerView;

namespace BMM.UI.Droid.Application.ItemDecorators;

public class ProfileAchievementsItemDecorator : RecyclerView.ItemDecoration
{
    private readonly int _spacing;

    public ProfileAchievementsItemDecorator(int spacing)
    {
        _spacing = spacing;
    }

    public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
    {
        int elementPosition = parent.GetChildAdapterPosition(view);
        object element = ((MvxRecyclerAdapter)parent.GetAdapter())!.GetItem(elementPosition);

        if (element is not AchievementPO)
            return;

        outRect.Left = _spacing;
        outRect.Top = _spacing;
    }
}