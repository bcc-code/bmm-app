using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ExternalRelationsPlayTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(ExternalRelationsPlayTableViewCell));

        public ExternalRelationsPlayTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ExternalRelationsPlayTableViewCell, IBibleStudyExternalRelationPO>();
                
                set.Bind(PlayLabel)
                    .To(po => po.Title);
                
                set.Bind(PlayButton)
                    .For(v => v.BindTap())
                    .To(po => po.ClickedCommand);
                
                set.Apply();
            });
        }

        private void SetThemes()
        {
            PlayLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }
    }
}