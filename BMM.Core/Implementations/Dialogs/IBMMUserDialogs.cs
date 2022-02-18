using System;
using Acr.UserDialogs;

namespace BMM.Core.Implementations.Dialogs
{
    public interface IBMMUserDialogs
    {
        IDisposable ActionSheet(ActionSheetConfig config);
    }
}