using System;

namespace BMM.Core.Implementations.Device
{
    public static class ApplicationStateWatcher
    {
        private static ApplicationState _state;
        public static Action<ApplicationState> ApplicationStateChanged;

        public static ApplicationState State
        {
            get => _state;
            set
            {
                _state = value;
                ApplicationStateChanged?.Invoke(_state);
            }
        }
    }
    
    public enum ApplicationState
    {
        Foreground = 0,
        Background = 1
    }
}