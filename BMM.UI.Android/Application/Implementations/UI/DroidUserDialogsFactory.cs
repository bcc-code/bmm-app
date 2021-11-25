using Acr.UserDialogs;
using BMM.Core.Implementations.UI;
using BMM.UI.iOS;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Implementations.UI
{
    public class DroidUserDialogsFactory : IUserDialogsFactory
    {
        public IUserDialogs Create()
        {
            var droidUserDialogs = new DroidUserDialogs(() => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
            return droidUserDialogs;
        }
    }
}