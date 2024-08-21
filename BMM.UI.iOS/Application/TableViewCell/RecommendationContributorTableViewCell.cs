using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Recommendations;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Helpers;
using BMM.UI.iOS.TableViewCell.Base;

namespace BMM.UI.iOS
{
    public partial class RecommendationContributorTableViewCell : BaseRecommendationTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(RecommendationContributorTableViewCell));

        public RecommendationContributorTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
        }

        protected override void Bind(MvxFluentBindingDescriptionSet<BaseRecommendationTableViewCell, RecommendationPO> set)
        {
            base.Bind(set);
            
            set.Bind(ContributorName)
                .To(d => d.ContributorPO.Contributor.Name);

            set.Bind(ContributorImage)
                .For(v => v.ImagePath)
                .To(vm => vm.ContributorPO.Contributor.Cover)
                .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.ArtistPlaceholderImage);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
            ContributorName.ApplyTextTheme(AppTheme.Title2);
        }

        protected override UILabel LabelTitle => TitleLabel;
        protected override UILabel LabelRemoteTitle => RemoteTitleLabel;
        protected override UILabel LabelRemoteSubtitle => RemoteSubitleLabel;
        protected override UIView ContainerRemoteTitle => RemoteTitleContainer;
        protected override NSLayoutConstraint ConstraintTitleToBottomView => TitleToBottomViewConstraint;
    }
}