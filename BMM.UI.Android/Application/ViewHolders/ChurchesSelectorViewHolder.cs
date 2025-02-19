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
using BMM.UI.Droid.Application.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class ChurchesSelectorViewHolder : MvxRecyclerViewHolder
    {
        private bool _isRightItemSelected;
        private bool _isLeftItemSelected;
        private TextView _leftItemLabel;
        private TextView _rightItemLabel;

        public ChurchesSelectorViewHolder(
            View itemView,
            IMvxAndroidBindingContext context) : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        private void Bind()
        {
            _leftItemLabel = ItemView.FindViewById<TextView>(ResourceConstant.Id.LeftItemLabel);
            _rightItemLabel = ItemView.FindViewById<TextView>(ResourceConstant.Id.RightItemLabel);
            
            var set = this.CreateBindingSet<ChurchesSelectorViewHolder, HvheChurchesSelectorPO>();

            set.Bind(this)
                .For(v => v.IsLeftItemSelected)
                .To(po => po.IsLeftItemSelected);
            
            set.Bind(this)
                .For(v => v.IsRightItemSelected)
                .To(po => po.IsRightItemSelected);
            
            set.Apply();
        }

        public bool IsRightItemSelected
        {
            get => _isRightItemSelected;
            set
            {
                _isRightItemSelected = value;
                _rightItemLabel.SetTextColor(_isRightItemSelected
                    ? ItemView.Context.GetColorFromResource(ResourceConstant.Color.label_one_color)
                    : ItemView.Context.GetColorFromResource(ResourceConstant.Color.label_three_color));
            }
        }

        public bool IsLeftItemSelected
        {
            get => _isLeftItemSelected;
            set
            {
                _isLeftItemSelected = value;
                _leftItemLabel.SetTextColor(_isLeftItemSelected
                    ? ItemView.Context.GetColorFromResource(ResourceConstant.Color.label_one_color)
                    : ItemView.Context.GetColorFromResource(ResourceConstant.Color.label_three_color));
            }
        }
    }
}