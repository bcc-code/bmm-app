using System;

namespace BMM.UI.iOS
{
    public interface IBaseViewController
    {
        void RegisterViewController(IBaseViewController viewController);

        bool IsVisible();

        string ViewModelName => string.Empty;
    }
}