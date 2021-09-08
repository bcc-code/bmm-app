using System;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class SupportEndedViewController : BaseViewController<SupportEndedViewModel>
    {
        public override Type ParentViewControllerType => null;

        public SupportEndedViewController()
            : base(nameof(SupportEndedViewController))
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<SupportEndedViewController, SupportEndedViewModel>();
            set.Bind(SupportEndedMessage).To(vm => vm.SupportEndedInfo);
            set.Bind(UpdateBmmBtn).To(vm => vm.ShowAppUpdatePageCommand);
            set.Bind(UpdateBmmBtn).For(v => v.Hidden).To(vm => vm.ShouldShowAppUpdateButton).WithConversion<InvertedVisibilityConverter>();
            set.Bind(UpdateBmmBtn).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.SupportEndedViewModel_UpdateButton);
            set.Apply();
        }
    }
}