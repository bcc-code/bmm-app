using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Base
{
    public abstract class BaseViewModel<T> : BaseViewModel, IMvxViewModel<T>
    {
        public abstract void Prepare(T parameter);
    }
}