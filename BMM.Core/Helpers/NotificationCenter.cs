using System;

namespace BMM.Core.Helpers
{
    public class NotificationCenter : INotificationCenter
    {
        public event AppLanguageChangedEventHandler AppLanguageChanged;

        public void RaiseAppLanguageChanged()
        {
            AppLanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}