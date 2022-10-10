using MvvmCross.ViewModels;

namespace BMM.Core.Models.POs.Base
{
    public interface ITrackListHolderPO : IMvxNotifyPropertyChanged
    {
        string Title { get; }

        string Cover { get; }
    }
}