using System;
using Acr.UserDialogs;
using BMM.Core.Implementations.Dialogs;
using BMM.UI.iOS.Extensions;

namespace BMM.UI.iOS.Implementations.Dialogs
{
    public class iOSBMMUserDialogs : IBMMUserDialogs
    {
        private readonly IUserDialogs _userDialogs;

        public iOSBMMUserDialogs(IUserDialogs userDialogs)
        {
            _userDialogs = userDialogs;
        }
        
        public IDisposable ActionSheet(ActionSheetConfig config)
        {
            foreach (var option in config.Options)
                option.ItemIcon = option.ItemIcon.ToStandardIosImageName();

            return _userDialogs.ActionSheet(config);
        }
    }
}