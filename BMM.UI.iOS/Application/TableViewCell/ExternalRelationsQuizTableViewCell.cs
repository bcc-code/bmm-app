using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class ExternalRelationsQuizTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(ExternalRelationsQuizTableViewCell));

        public ExternalRelationsQuizTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ExternalRelationsQuizTableViewCell, IBibleStudyExternalRelationPO>();
                
                set.Bind(TitleLabel)
                    .To(po => po.Title);
                
                set.Bind(SubtitleLabel)
                    .To(po => po.Subtitle);
                
                set.Bind(ActionButton)
                    .To(po => po.ClickedCommand);
                
                set.Apply();
            });
        }

        private void SetThemes()
        {
            ActionButton.ApplyButtonStyle(AppTheme.ButtonPrimarySmall);
            TitleLabel.ApplyTextTheme(AppTheme.Subtitle2Label3);
            SubtitleLabel.ApplyTextTheme(AppTheme.Title2);
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }
    }
}