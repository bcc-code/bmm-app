using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidX.CardView.Widget;
using BMM.Core.Models.POs.YearInReview;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.ValueConverters;
using FFImageLoading.Cross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class YearInReviewItemViewHolder : MvxRecyclerViewHolder
    {
        private readonly double _itemWidth;
        private readonly double _itemHeight;
        private readonly double _imageWidth;
        private readonly double _imageHeight;
        private MvxCachedImageView _yearInReviewImageView;
        private CardView _yearInReviewCardView;
        private Color _shadowColor;

        private MvxCachedImageView YearInReviewImageView =>
            _yearInReviewImageView ??= ItemView.FindViewById<MvxCachedImageView>(Resource.Id.YearInReviewImageView);
        
        private CardView YearInReviewCardView =>
            _yearInReviewCardView ??= ItemView.FindViewById<CardView>(Resource.Id.YearInReviewCardView);

        public YearInReviewItemViewHolder(
            View itemView,
            IMvxAndroidBindingContext context,
            double itemWidth,
            double itemHeight,
            double imageWidth,
            double imageHeight) : base(itemView, context)
        {
            _itemWidth = itemWidth;
            _itemHeight = itemHeight;
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
            this.DelayBind(Bind);
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<YearInReviewItemViewHolder, YearInReviewItemPO>();
            
            set.Bind(this)
                .For(v => v.ShadowColor)
                .To(po => po.Color)
                .WithConversion<HexToColorValueConverter>();
            
            set.Apply();
            YearInReviewImageView!.ClipToOutline = true;
        }

        public Color ShadowColor
        {
            get => _shadowColor;
            set
            {
                _shadowColor = value;
                
                YearInReviewImageView.SetBackgroundColor(_shadowColor);

                if (Build.VERSION.SdkInt < BuildVersionCodes.P)
                    return;
                
                YearInReviewCardView.SetOutlineAmbientShadowColor(_shadowColor);
                YearInReviewCardView.SetOutlineSpotShadowColor(_shadowColor);
            }
        }

        public void Update()
        {
            ItemView.UpdateWidth((int)_itemWidth);
            ItemView.UpdateHeight((int)_itemHeight);
            YearInReviewCardView.UpdateWidth((int)_imageWidth);
            YearInReviewCardView.UpdateHeight((int)_imageHeight);
        }
    }
}