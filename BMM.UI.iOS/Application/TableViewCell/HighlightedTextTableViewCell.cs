﻿using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class HighlightedTextTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(HighlightedTextTableViewCell));

        public HighlightedTextTableViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<HighlightedTextTableViewCell, HighlightedTextPO>();
                
                set.Bind(HighlightedTextLabel)
                    .For(v => v.StyledTextContainer)
                    .To(po => po.StyledTextContainer);

                set.Bind(PositionLabel)
                    .To(po => po.StartPositionInMs)
                    .WithConversion<MillisecondsToTimeValueConverter>();

                set.Bind(ShareButton)
                    .To(po => po.ShareCommand);
                
                set.Bind(ContentView)
                    .For(v => v.BindTap())
                    .To(po => po.ItemClickedCommand);
                
                set.Apply();
                PositionLabel.ApplyTextTheme(AppTheme.Paragraph3Label1);
            });
        }
        
        protected override bool HasHighlightEffect => false;
    }
}