using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using Foundation;
using System;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ContributorTableViewCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("ContributorTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("ContributorTableViewCell");

        public ContributorTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                CoverImageView.ErrorAndLoadingPlaceholderImagePath(IosConstants.ArtistPlaceholderImage);
                var set = this.CreateBindingSet<ContributorTableViewCell, CellWrapperViewModel<Contributor>>();
                set.Bind(TitleLabel).To(vm => vm.Item.Name);
                set.Bind(OptionsButton).WithConversion<OptionButtonCommandValueConverter>();
                set.Bind(CoverImageView)
                    .For(v => v.ImagePath)
                    .To(vm => vm.Item.Cover)
                    .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.ArtistPlaceholderImage);
                set.Apply();
            });
        }

        public static ContributorTableViewCell Create()
        {
            return (ContributorTableViewCell)Nib.Instantiate(null, null)[0];
        }
    }
}