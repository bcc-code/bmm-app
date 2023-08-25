using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ExternalRelationsOpenWithSubtitleTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(ExternalRelationsOpenWithSubtitleTableViewCell));

        public ExternalRelationsOpenWithSubtitleTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ExternalRelationsOpenWithSubtitleTableViewCell, IBibleStudyExternalRelationPO>();
                
                set.Bind(OpenLabel)
                    .To(po => po.Title);
                
                set.Bind(SubtitleLabel)
                    .To(po => po.Subtitle);
                
                set.Bind(OpenButton)
                    .For(v => v.BindTap())
                    .To(po => po.ClickedCommand);
                
                set.Apply();
            });
        }

        private void SetThemes()
        {
            OpenLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            SubtitleLabel.ApplyTextTheme(AppTheme.Paragraph2Label3);
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }
    }
}