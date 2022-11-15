using System;
using System.Linq;
using Acr.UserDialogs;
using BMM.Core.Implementations.Dialogs;

namespace BMM.UI.Droid.Application.Implementations.Dialogs
{
    public class DroidBMMUserDialogs : IBMMUserDialogs
    {
        private readonly IUserDialogs _userDialogs;

        public DroidBMMUserDialogs(IUserDialogs userDialogs)
        {
            _userDialogs = userDialogs;
        }
        
        public IDisposable ActionSheet(ActionSheetConfig config)
        {
            foreach (var option in config.Options.Where(o => !string.IsNullOrEmpty(o.ItemIcon)))
                option.ItemIcon = option.ItemIcon.ToLower();

            return _userDialogs.ActionSheet(config);
        }
    }
}