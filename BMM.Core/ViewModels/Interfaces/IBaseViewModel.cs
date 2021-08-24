using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IBaseViewModel : IMvxNotifyPropertyChanged
    {
        IMvxAsyncCommand CloseCommand { get; }
        bool IsLoading { get; set; }
    }
}