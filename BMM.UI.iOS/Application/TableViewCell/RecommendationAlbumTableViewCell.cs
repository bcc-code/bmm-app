using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Recommendations;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Helpers;
using BMM.UI.iOS.TableViewCell.Base;

namespace BMM.UI.iOS
{
    public partial class RecommendationAlbumTableViewCell : BaseRecommendationTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(RecommendationAlbumTableViewCell));

        public RecommendationAlbumTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
        }

        protected override void Bind(MvxFluentBindingDescriptionSet<BaseRecommendationTableViewCell, RecommendationPO> set)
        {
            base.Bind(set);
            
            set.Bind(AlbumName)
                .To(d => d.TrackListHolder.Title);

            set.Bind(AlbumImage)
                .For(v => v.ImagePath)
                .To(vm => vm.TrackListHolder.Cover)
                .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.CoverPlaceholderImage);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
            AlbumName.ApplyTextTheme(AppTheme.Title2);
        }

        protected override UILabel LabelTitle => TitleLabel;
        protected override UILabel LabelRemoteTitle => RemoteTitleLabel;
        protected override UILabel LabelRemoteSubtitle => RemoteSubtitleLabel;
        protected override UIView ContainerRemoteTitle => RemoteTitleContainer;
        protected override NSLayoutConstraint ConstraintTitleToBottomView => TitleToBottomViewConstraint;
    }
}