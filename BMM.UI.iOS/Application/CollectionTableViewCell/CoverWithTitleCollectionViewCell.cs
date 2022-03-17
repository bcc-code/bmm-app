using System;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class CoverWithTitleCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(CoverWithTitleCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(CoverWithTitleCollectionViewCell), NSBundle.MainBundle);

        public CoverWithTitleCollectionViewCell(IntPtr handle): base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<CoverWithTitleCollectionViewCell, CellWrapperViewModel<ITrackListDisplayable>>();

                set.Bind(TitleLabel).To(vm => vm.Item.Title);
                ImageView.ErrorAndLoadingPlaceholderImagePathForCover();
                set.Bind(ImageView)
                    .For(v => v.ImagePath)
                    .To(vm => vm.Item.Cover)
                    .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.CoverPlaceholderImage);

                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            ImageView.Layer.CornerRadius = 16f;
            ImageView.ClipsToBounds = true;
            TitleLabel.ApplyTextTheme(AppTheme.Subtitle3Label1);
        }
    }
}