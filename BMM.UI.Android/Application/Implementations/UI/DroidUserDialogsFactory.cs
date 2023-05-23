using Acr.UserDialogs;
using BMM.Core.Implementations.UI;
using BMM.UI.iOS;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Implementations.UI
{
    public class DroidUserDialogsFactory : IUserDialogsFactory
    {
        private readonly IMvxAndroidCurrentTopActivity _mvxAndroidCurrentTopActivity;

        public DroidUserDialogsFactory(IMvxAndroidCurrentTopActivity mvxAndroidCurrentTopActivity)
        {
            _mvxAndroidCurrentTopActivity = mvxAndroidCurrentTopActivity;
        }
        
        public IUserDialogs Create()
        {
            var droidUserDialogs = new DroidUserDialogs(() => _mvxAndroidCurrentTopActivity.Activity);
            return droidUserDialogs;
        }
    }
}