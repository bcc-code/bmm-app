using BMM.Core.ViewModels;
using BMM.UI.iOS.ViewController.Base;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace BMM.UI.iOS.ViewController
{
    [MvxModalPresentation(WrapInNavigationController = true)]
    public partial class WebBrowserViewController : WebBrowserBaseViewController<WebBrowserViewModel>
    {
        public WebBrowserViewController() : base(nameof(WebBrowserViewController))
        {
        }

        public override Type ParentViewControllerType => null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
        
        protected override UIView WebBrowserContainer => viewContainer;

        protected override string Script => ViewModel?.Script;
    }
}