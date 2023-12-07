using Acr.UserDialogs;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Listeners;
using FFImageLoading.Cross;
using FFImageLoading.Extensions;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using LinearLayout = Android.Widget.LinearLayout;
using Orientation = Android.Widget.Orientation;

namespace BMM.UI.Droid.Application.ViewHolders;

public class ProjectBoxExpandedViewHolder : ProjectBoxViewHolder
{
    private IBmmObservableCollection<IAchievementPO> _achievements;

    public ProjectBoxExpandedViewHolder(View itemView, IMvxAndroidBindingContext context, PodcastContextHeaderRecyclerAdapter podcastContextHeaderRecyclerAdapter) : base(itemView, context, podcastContextHeaderRecyclerAdapter)
    {
    }

    protected override void Bind(MvxFluentBindingDescriptionSet<ProjectBoxViewHolder, ProjectBoxPO> set)
    {
        base.Bind(set);

        set.Bind(this)
            .For(v => v.Achievements)
            .To(po => po.Achievements);
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
        if (achievements == null ||achievements.Count == 0)
            return;
        
        var rows = achievements.OfType<IBasePO>().Chunk(4).ToList();
        var toFillInLastRow = 4 - rows.Last().Length;
        var lastRow = rows.Last().ToList();
        
        for (int i = 0; i < toFillInLastRow; i++)
            lastRow.Add(new EmptyPO());

        rows[^1] = lastRow.ToArray();
            
        var achievementsLayout = ItemView.FindViewById<LinearLayout>(Resource.Id.AchievementsLayout);
        achievementsLayout.RemoveAllViews();
        
        foreach (var row in rows)
        {
            var linearLayout = new LinearLayout(achievementsLayout.Context);
            linearLayout.Orientation = Orientation.Horizontal;
            linearLayout.SetGravity(GravityFlags.End);

            foreach (var rowItem in row)
            {
                if (rowItem is IAchievementPO achievementPO)
                {
                    var image = new MvxCachedImageView(achievementsLayout.Context);
                    image.ImagePath = achievementPO.ImagePath;
                    image.SetOnClickListener(new ClickListener(() => achievementPO.AchievementClickedCommand?.Execute()));

                    var lp = new LinearLayout.LayoutParams(40.DpToPixels(), 40.DpToPixels());
                    lp.SetMargins(0, 6.DpToPixels(), 6.DpToPixels(), 0); 
                    image.LayoutParameters = lp;
                    linearLayout.AddView(image);
                }
                else
                {
                    var space = new Space(achievementsLayout.Context);
                    var lp = new LinearLayout.LayoutParams(40.DpToPixels(), 40.DpToPixels());
                    lp.SetMargins(0, 6.DpToPixels(), 6.DpToPixels(), 0);
                    space.LayoutParameters = lp;
                    linearLayout.AddView(space);
                }
            }

            achievementsLayout.AddView(linearLayout);
        }
    }
}