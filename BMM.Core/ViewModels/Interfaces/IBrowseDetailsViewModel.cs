using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IBrowseDetailsViewModel : IMvxViewModel<IBrowseDetailsParameters>
    {
        string Title { get; }
    }
}