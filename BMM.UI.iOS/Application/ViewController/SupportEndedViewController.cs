using System;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;

namespace BMM.UI.iOS
{
    public partial class SupportEndedViewController : BaseViewController<SupportEndedViewModel>
    {
        public override Type ParentViewControllerType => null;

        public SupportEndedViewController()
            : base("SupportEndedViewController")
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<SupportEndedViewController, SupportEndedViewModel>();
            set.Bind(SupportEndedMessage).To(vm => vm.SupportEndedInfo);
            set.Bind(UpdateBmmBtn).To(vm => vm.ShowAppUpdatePageCommand);
            set.Bind(UpdateBmmBtn).For(v => v.Hidden).To(vm => vm.ShouldShowAppUpdateButton).WithConversion<InvertedVisibilityConverter>();
            set.Bind(UpdateBmmBtn).For("Title").To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("UpdateButton");
            set.Apply();
        }
    }
}