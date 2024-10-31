using Android.Views;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.UI.Droid.Application.Listeners;
using FFImageLoading.Cross;
using FFImageLoading.Extensions;
using Microsoft.Maui.Devices;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using LinearLayout = Android.Widget.LinearLayout;

namespace BMM.UI.Droid.Application.ViewHolders;

public class GibraltarProjectBoxExpandedViewHolder : MvxRecyclerViewHolder
{
    private const int ImageSize = 64;
    private const int Spacing = 16;
    private IBmmObservableCollection<IAchievementPO> _achievements;

    public GibraltarProjectBoxExpandedViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
    {
        this.DelayBind(Bind);
    }

    private void Bind()
    {
        var set = this.CreateBindingSet<GibraltarProjectBoxExpandedViewHolder, ProjectBoxPO>();

        set.Bind(this)
            .For(v => v.Achievements)
            .To(po => po.Achievements);

        set.Apply();
    }

    public IBmmObservableCollection<IAchievementPO> Achievements
    {
        get => _achievements;
        set
        {
            _achievements = value;
            SetItems(_achievements);
        }
    }

    private void SetItems(IBmmObservableCollection<IAchievementPO> achievements)
    {
        if (achievements == null || achievements.Count == 0)
            return;

        var achievementsLayout = ItemView.FindViewById<LinearLayout>(Resource.Id.AchievementsLayout);
        achievementsLayout.RemoveAllViews();

        ItemView.Post(() =>
        {
            int imageSize = ImageSize.DpToPixels();
            int imageCount = achievements.Take(4).Count();
            int layoutWidth = achievementsLayout.Width;

            int spacing;
            if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                spacing = Spacing.DpToPixels();
                int totalImageWidth = imageSize * imageCount + spacing * (imageCount - 1);
                int offset = (layoutWidth - totalImageWidth) / 2;

                achievementsLayout.SetPadding(offset, 0, offset, 0);
            }
            else
            {
                int totalImageWidth = imageSize * imageCount;
                int remainingSpace = layoutWidth - totalImageWidth;
                spacing = remainingSpace > 0 ? remainingSpace / (imageCount - 1) : 0;
                achievementsLayout.SetPadding(0, 0, 0, 0);
            }

            for (int i = 0; i < imageCount; i++)
            {
                var item = achievements[i];
                var image = new MvxCachedImageView(achievementsLayout.Context);
                image.ImagePath = item.ImagePath;
                image.SetOnClickListener(new ClickListener(() => item.AchievementClickedCommand?.Execute()));

                var lp = new LinearLayout.LayoutParams(imageSize, imageSize);

                if (i != 0)
                    lp.SetMargins(spacing, 0, 0, 0);

                image.LayoutParameters = lp;
                achievementsLayout.AddView(image);
            }
        });
    }
}