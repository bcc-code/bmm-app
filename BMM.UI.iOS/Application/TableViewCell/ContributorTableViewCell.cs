using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;

namespace BMM.UI.iOS
{
    public partial class ContributorTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ContributorTableViewCell));

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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
        }
    }
}