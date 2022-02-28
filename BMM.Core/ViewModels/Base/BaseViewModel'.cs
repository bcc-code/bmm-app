using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.ViewModels.Base
{
    public abstract class BaseViewModel<T> : BaseViewModel, IBaseViewModel<T>
    {
        public void Prepare(T parameter)
        {
            NavigationParameter = parameter;
        }

        public T NavigationParameter { get; private set; }
    }
}