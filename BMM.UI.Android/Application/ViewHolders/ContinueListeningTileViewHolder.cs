using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.CustomViews;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.ValueConverters;
using FFImageLoading.Cross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Plugin.Color.Platforms.Android.Binding;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class ContinueListeningTileViewHolder : MvxRecyclerViewHolder
    {
        private DateTime? _date;
        private TextView _remainingLabel;
        private ProgressBarView _progressBarView;

        public ContinueListeningTileViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            this.DelayBind(Bind);
        }

        protected ContinueListeningTileViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        private void Bind()
        {
            _remainingLabel = ItemView.FindViewById<TextView>(Resource.Id.RemainingLabel);
            _progressBarView = ItemView.FindViewById<ProgressBarView>(Resource.Id.ProgressBarView);
            var backgroundView = ItemView.FindViewById<ConstraintLayout>(Resource.Id.BackgroundView);
            var imageView = ItemView.FindViewById<MvxCachedImageView>(Resource.Id.CoverImageView);
            imageView!.ClipToOutline = true;
            
            var set = this.CreateBindingSet<ContinueListeningTileViewHolder, CellWrapperViewModel<ContinueListeningTile>>();

            set.Bind(backgroundView)
                .For(v => v.BindBackgroundColor())
                .To(po => po.Item.BackgroundColor)
                .WithConversion<HexToColorValueConverter>(ItemView.Context.GetColorFromResource(Resource.Color.tile_default_color));

            set.Bind(this)
                .For(v => v.Date)
                .To(po => po.Item.Date);
            
            set.Apply();
        }

        public DateTime? Date
        {
            get => _date;
            set
            {
                _date = value;
                var progressBarVisibility = _date == null
                    ? ViewStates.Visible
                    : ViewStates.Gone;

                _progressBarView.Visibility = progressBarVisibility;
                _remainingLabel.Visibility = progressBarVisibility;
            }
        }
    }
}