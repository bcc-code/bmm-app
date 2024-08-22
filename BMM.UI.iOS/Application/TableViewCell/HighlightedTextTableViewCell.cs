using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views.Gestures;

namespace BMM.UI.iOS
{
    public partial class HighlightedTextTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(HighlightedTextTableViewCell));

        public HighlightedTextTableViewCell(ObjCRuntime.NativeHandle handle) : base(handle)
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
                
                set.Bind(ShareButtonArea)
                    .For(v => v.BindTap())
                    .To(po => po.ShareCommand);
                
                set.Bind(ContentView)
                    .For(v => v.BindTap())
                    .To(po => po.ItemClickedCommand);

                set.Bind(PlayIcon)
                    .For(v => v.BindVisible())
                    .To(po => po.IsSong)
                    .WithConversion<InvertedBoolConverter>();
                
                set.Bind(PositionLabel)
                    .For(v => v.BindVisible())
                    .To(po => po.IsSong)
                    .WithConversion<InvertedBoolConverter>();

                set.Bind(SuggestEditButton)
                    .For(v => v.BindTitle())
                    .To(po => po.TextSource[Translations.HighlightedTextTrackViewModel_SuggestEdit]);

                set.Bind(SuggestEditButton)
                    .To(po => po.SuggestEditClickedCommand);

                set.Bind(SuggestEditButton)
                    .For(v => v.BindVisible())
                    .To(vm => vm.IsSong)
                    .WithConversion<InvertedBoolConverter>();
                
                set.Apply();
                PositionLabel.ApplyTextTheme(AppTheme.Paragraph3Label1);
                SuggestEditButton.ApplyButtonStyle(AppTheme.ButtonTertiarySmall);
                ShareButtonArea!.Layer.BorderWidth = 1.0f;
                ShareButtonArea.Layer.BorderColor = AppColors.SeparatorColor.CGColor;
                ShareButtonArea.Layer.CornerRadius = ShareButtonArea.Frame.Height * 0.5f;
            });
        }
        
        protected override bool HasHighlightEffect => false;
    }
}