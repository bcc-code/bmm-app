using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Other;
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

        public DiscoverSectionHeaderTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<DiscoverSectionHeaderTableViewCell, DiscoverSectionHeaderPO>();
                set.Bind(Titel).To(po => po.DiscoverSectionHeader.Title);
                set.Bind(LinkButton).For(v => v.BindTitle()).To(po => po.TextSource).WithConversion<MvxLanguageConverter>(Translations.ExploreNewestViewModel_ShowAll);
                set.Bind(LinkButton).For(v => v.Hidden).To(po => po.HasLink).WithConversion<VisibilityConverter>();
                set.Bind(LinkButton).To(po => po.DeepLinkButtonClickedCommand);
                set.Bind(Divider)
                    .For(v => v.BindVisible())
                    .To(po => po.IsSeparatorVisible);
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