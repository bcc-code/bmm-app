using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Recommendations;
using BMM.Core.Translation;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.TableViewCell.Base;

namespace BMM.UI.iOS
{
    public partial class RecommendationTrackTableViewCell : BaseRecommendationTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(RecommendationTrackTableViewCell));

        public RecommendationTrackTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<RecommendationTrackTableViewCell, RecommendationPO>();

                set.Bind(TitleLabel)
                    .To(d => d.TextSource[Translations.ExploreNewestViewModel_Recommended]);

                set.Apply();
            });
        }

        protected override void Bind(MvxFluentBindingDescriptionSet<BaseRecommendationTableViewCell, RecommendationPO> set)
        {
            base.Bind(set);
                            
            set.Bind(TrackTitleLabel)
                .To(d => d.TrackPO.TrackTitle);

            set.Bind(TrackSubtitleLabel)
                .To(d => d.TrackPO.TrackSubtitle);
                
            set.Bind(TrackMetaLabel)
                .To(d => d.TrackPO.TrackMeta);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
            TrackTitleLabel.ApplyTextTheme(AppTheme.Title2);
            TrackSubtitleLabel.ApplyTextTheme(AppTheme.Subtitle3Label2);
            TrackMetaLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }

        protected override UILabel LabelTitle => TitleLabel;
        protected override UILabel LabelRemoteTitle => RemoteTitleLabel;
        protected override UILabel LabelRemoteSubtitle => RemoteSubtitleLabel;
        protected override UIView ContainerRemoteTitle => RemoteTitleContainer;
        protected override NSLayoutConstraint ConstraintTitleToBottomView => TitleToBottomViewConstraint;
    }
}