using System;
using _Microsoft.Android.Resource.Designer;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.YearInReview;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Extensions;
using FFImageLoading.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class HighlightedChurchViewHolder : MvxRecyclerViewHolder
    {
        private View _rightPointsContainer;
        private View _middlePointsContainer;
        private View _leftPointsContainer;
        private GameNights _firstGameNight;
        private GameNights _secondGameNight;
        private GameNights _thirdGameNight;
        private TextView _rightPointsLabel;
        private TextView _middlePointsLabel;
        private TextView _leftPointsLabel;
        private static readonly int PointsContainerRadius = 12.DpToPixels();
        private static readonly int PointsContainerSmallRadius = 4.DpToPixels();
        public HighlightedChurchViewHolder(
            View itemView,
            IMvxAndroidBindingContext context) : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        private void Bind()
        {
            _rightPointsContainer = ItemView.FindViewById<View>(ResourceConstant.Id.RightPointsContainer);
            _rightPointsLabel = ItemView.FindViewById<TextView>(ResourceConstant.Id.RightPointsLabel);
            _middlePointsContainer = ItemView.FindViewById<View>(ResourceConstant.Id.MiddlePointsContainer);
            _middlePointsLabel = ItemView.FindViewById<TextView>(ResourceConstant.Id.MiddlePointsLabel);
            _leftPointsContainer = ItemView.FindViewById<View>(ResourceConstant.Id.LeftPointsContainer);
            _leftPointsLabel = ItemView.FindViewById<TextView>(ResourceConstant.Id.LeftPointsLabel);
            var boysPointsContainer = ItemView.FindViewById<View>(ResourceConstant.Id.BoysPointsContainer);
            var girlsPointsContainer = ItemView.FindViewById<View>(ResourceConstant.Id.GirlsPointsContainer);
            
            boysPointsContainer.ApplyRoundedCorners(PointsContainerSmallRadius, PointsContainerRadius, PointsContainerSmallRadius, PointsContainerSmallRadius);
            girlsPointsContainer.ApplyRoundedCorners(PointsContainerSmallRadius, PointsContainerSmallRadius, PointsContainerSmallRadius, PointsContainerRadius);
            
            var set = this.CreateBindingSet<HighlightedChurchViewHolder, HvheChurchPO>();

            set.Bind(this)
                .For(v => v.FirstGameNight)
                .To(po => po.FirstGameNight);
            
            set.Bind(this)
                .For(v => v.SecondGameNight)
                .To(po => po.SecondGameNight);
            
            set.Bind(this)
                .For(v => v.ThirdGameNight)
                .To(po => po.ThirdGameNight);
            
            set.Apply();
        }
        
        public GameNights FirstGameNight
        {
            get => _firstGameNight;
            set
            {
                _firstGameNight = value;
                _leftPointsContainer.SetBackgroundColor(GetBackgroundColor(_firstGameNight));
                _leftPointsLabel.SetTextColor(GetTextColor(_firstGameNight));
                _leftPointsContainer.ApplyRoundedCorners(PointsContainerRadius, PointsContainerRadius, PointsContainerRadius, PointsContainerRadius);
            }
        }

        public GameNights SecondGameNight
        {
            get => _secondGameNight;
            set
            {
                _secondGameNight = value;
                _middlePointsContainer.SetBackgroundColor(GetBackgroundColor(_secondGameNight));
                _middlePointsLabel.SetTextColor(GetTextColor(_secondGameNight));
                _middlePointsContainer.ApplyRoundedCorners(PointsContainerRadius, PointsContainerRadius, PointsContainerRadius, PointsContainerRadius);
            }
        }
        
        public GameNights ThirdGameNight
        {
            get => _thirdGameNight;
            set
            {
                _thirdGameNight = value;
                _rightPointsContainer.SetBackgroundColor(GetBackgroundColor(_thirdGameNight));
                _rightPointsLabel.SetTextColor(GetTextColor(_thirdGameNight));
                _rightPointsContainer.ApplyRoundedCorners(PointsContainerRadius, PointsContainerRadius, PointsContainerRadius, PointsContainerRadius);
            }
        }
        
        private Color GetBackgroundColor(GameNights gameNight)
        {
            int colorId = gameNight switch
            {
                GameNights.Boys => ResourceConstant.Color.boys_color,
                GameNights.Girls => ResourceConstant.Color.girls_color,
                _ => ResourceConstant.Color.background_one_color
            };

            return ItemView.Context.GetColorFromResource(colorId);
        }
        
        private Color GetTextColor(GameNights gameNight)
        {
            int colorId = gameNight switch
            {
                GameNights.Boys or GameNights.Girls => ResourceConstant.Color.global_white_one,
                _ => ResourceConstant.Color.label_three_color
            };

            return ItemView.Context.GetColorFromResource(colorId);
        }
    }
}