using System;
using _Microsoft.Android.Resource.Designer;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.YearInReview;
using BMM.UI.Droid.Application.Adapters;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class BoysVsGirlsViewHolder : MvxRecyclerViewHolder
    {
        private const string FontName = "IBMPlexMono-Bold.ttf";

        public BoysVsGirlsViewHolder(
            View itemView,
            IMvxAndroidBindingContext context) : base(itemView, context)
        {
        }

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            var boysPointLabel = ItemView.FindViewById<TextView>(ResourceConstant.Id.BoysPointsLabel);
            var girlsPointsLabel = ItemView.FindViewById<TextView>(ResourceConstant.Id.GirlsPointsLabel);
            var girlsLabel = ItemView.FindViewById<TextView>(ResourceConstant.Id.GirlsLabel);
            var boysLabel = ItemView.FindViewById<TextView>(ResourceConstant.Id.BoysLabel);
            
            var customFont = Typeface.CreateFromAsset(ItemView.Context!.Assets, FontName); 
            boysPointLabel.Typeface = customFont;
            girlsPointsLabel.Typeface = customFont;
            girlsLabel.Typeface = customFont;
            boysLabel.Typeface = customFont;
        }
    }
}