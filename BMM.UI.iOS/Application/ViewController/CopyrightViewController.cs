using BMM.Core.ViewModels;

namespace BMM.UI.iOS
{
    public partial class CopyrightViewController : BaseViewController<CopyrightViewModel>
    {
        public CopyrightViewController()
            : base(nameof(CopyrightViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.CopyrightTextView.TextContainerInset = new UIKit.UIEdgeInsets(15, 15, 15, 15);
        }
    }
}