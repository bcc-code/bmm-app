using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IBaseViewModel<TParameter> : IBaseViewModel, IMvxViewModel<TParameter>
    {
        TParameter NavigationParameter { get; }
    }
}