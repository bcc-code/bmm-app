using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.YearInReview.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IYearInReviewViewModel : IBaseViewModel
    {
        IBmmObservableCollection<IYearInReviewItemPO> YearInReviewElements { get; }
        IMvxAsyncCommand ShareCommand { get; }
        string Description { get; }
        int CurrentPosition { get; set; }
    }
}