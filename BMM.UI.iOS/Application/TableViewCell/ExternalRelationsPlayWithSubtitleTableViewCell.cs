using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.TableViewCell.Base;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class ExternalRelationsPlayWithSubtitleTableViewCell : BaseExternalRelationsPlayTableViewCell
    {
        public static readonly NSString Key = new(nameof(ExternalRelationsPlayWithSubtitleTableViewCell));

        public ExternalRelationsPlayWithSubtitleTableViewCell(IntPtr handle) : base(handle)
        {
        }

        protected override void Bind(MvxFluentBindingDescriptionSet<BaseExternalRelationsPlayTableViewCell, IBibleStudyExternalRelationPO> set)
        {
            base.Bind(set);
            set.Bind(SubtitleLabel)
                .To(po => po.Subtitle);
        }

        protected override void SetThemes()
        {
            base.SetThemes();
            SubtitleLabel.ApplyTextTheme(AppTheme.Paragraph2Label3);
        }

        protected override bool HasHighlightEffect => false;
        protected override UILabel LabelPlay => PlayLabel;
        protected override UIView ButtonPlay => PlayButton;
        protected override UIImageView IconPlay => PlayIcon;
        protected override UIView ViewAnimation => AnimationView;
    }
}