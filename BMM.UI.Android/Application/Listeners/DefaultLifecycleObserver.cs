using AndroidX.Lifecycle;
using BMM.Core.Implementations.Device;

namespace BMM.UI.Droid.Application.Listeners
{
    public class DefaultLifecycleObserver : Java.Lang.Object, IDefaultLifecycleObserver, ILifecycleObserver
    {
        public void OnCreate(ILifecycleOwner p0)
        {
        }

        public void OnDestroy(ILifecycleOwner p0)
        {
        }

        public void OnPause(ILifecycleOwner p0)
        {
        }

        public void OnResume(ILifecycleOwner p0)
        {
        }

        public void OnStart(ILifecycleOwner p0)
        {
            ApplicationStateWatcher.State = ApplicationState.Foreground;
        }

        public void OnStop(ILifecycleOwner p0)
        {
            ApplicationStateWatcher.State = ApplicationState.Background;
        }
    }
}