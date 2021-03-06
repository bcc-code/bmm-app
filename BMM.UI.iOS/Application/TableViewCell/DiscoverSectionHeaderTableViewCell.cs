using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class DiscoverSectionHeaderTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(DiscoverSectionHeaderTableViewCell));

        public DiscoverSectionHeaderTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<DiscoverSectionHeaderTableViewCell, CellWrapperViewModel<DiscoverSectionHeader>>();
                set.Bind(Titel).To(vm => vm.Item.Title);
                set.Bind(LinkButton).For(v => v.BindTitle()).To(vm => vm.ViewModel.TextSource).WithConversion<MvxLanguageConverter>(Translations.ExploreNewestViewModel_ShowAll);
                set.Bind(LinkButton).WithConversion<DeepLinkButtonCommandValueConverter>();
                set.Bind(LinkButton).For(v => v.Hidden).To(vm => vm.Item.HasLink).WithConversion<VisibilityConverter>();
                set.Bind(Divider)
                    .For(v => v.BindVisible())
                    .To(vm => vm.Item.IsSeparatorVisible);
                set.Apply();
            });
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            Titel.ApplyTextTheme(AppTheme.Heading3);
            LinkButton.ApplyButtonStyle(AppTheme.ButtonSecondarySmall);
        }
    }
}