using System;

namespace BMM.UI.iOS
{
    public interface IBaseViewController
    {
        Type ParentViewControllerType { get; }

        ContainmentViewController ContainmentVC { get; set; }

        void RegisterViewController(IBaseViewController viewController);

        bool IsVisible();

        string ViewModelName => string.Empty;
    }
}