using System;
using BMM.Core.Implementations;
using Foundation;

namespace BMM.UI.iOS.Implementations
{
    public class BottomNavigationLoadedDependentExecutor : IUiDependentExecutor
    {
        private bool _isReady;

        // right now this can only be used for one action. If we need in more cases we have to convert this into a IList<Action>
        private Action _actionWhenReady;

        public BottomNavigationLoadedDependentExecutor()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(MenuViewController.MenuLoadedNotification,
                notification =>
                {
                    _isReady = true;
                    _actionWhenReady?.Invoke();
                    _actionWhenReady = null;
                });
        }

        public void ExecuteWhenReady(Action action)
        {
            if (_isReady)
                action.Invoke();
            else
                _actionWhenReady = action;
        }
    }
}