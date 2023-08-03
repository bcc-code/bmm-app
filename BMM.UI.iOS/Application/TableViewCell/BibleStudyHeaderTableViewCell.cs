using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.Other;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class BibleStudyHeaderTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(BibleStudyHeaderTableViewCell));

        public BibleStudyHeaderTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<BibleStudyHeaderTableViewCell, BibleStudyHeaderPO>();
                
                set.Bind(ThemeNameLabel)
                    .To(po => po.ThemeName);

                set.Bind(EpisodeTitleLabel)
                    .To(po => po.EpisodeTitle);

                set.Bind(EpisodeDateLabel)
                    .To(po => po.EpisodeDate);
                
                set.Apply();
            });
        }

        private void SetThemes()
        {
            ThemeNameLabel.ApplyTextTheme(AppTheme.Subtitle1Label2);
            EpisodeTitleLabel.ApplyTextTheme(AppTheme.Heading2);
            EpisodeDateLabel.ApplyTextTheme(AppTheme.Subtitle3Label2);
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }
    }
}