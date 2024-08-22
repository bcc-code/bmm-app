using BMM.UI.iOS.TableViewCell.Base;

namespace BMM.UI.iOS
{
    public partial class ExternalRelationsPlayTableViewCell : BaseExternalRelationsPlayTableViewCell
    {
        public static readonly NSString Key = new(nameof(ExternalRelationsPlayTableViewCell));

        public ExternalRelationsPlayTableViewCell(ObjCRuntime.NativeHandle handle) : base(handle)
        {
        }

        protected override UILabel LabelPlay => PlayLabel;
        protected override UIView ButtonPlay => PlayButton;
        protected override UIImageView IconPlay => PlayIcon;
        protected override UIView ViewAnimation => AnimationView;
    }
}