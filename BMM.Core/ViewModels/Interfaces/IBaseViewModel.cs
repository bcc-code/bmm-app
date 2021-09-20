using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IBaseViewModel : IMvxViewModel, IMvxNotifyPropertyChanged
    {
        IMvxAsyncCommand CloseCommand { get; }
        bool IsLoading { get; set; }
    }
}