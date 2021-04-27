using System;

namespace BMM.Core
{
    public delegate void AppLanguageChangedEventHandler(object sender, EventArgs e);

    public interface INotificationCenter
    {
        event AppLanguageChangedEventHandler AppLanguageChanged;

        void RaiseAppLanguageChanged();
    }
}