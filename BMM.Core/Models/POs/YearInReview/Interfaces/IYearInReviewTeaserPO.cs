using BMM.Core.Interactions.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.Models.POs.YearInReview.Interfaces
{
    public interface IYearInReviewTeaserPO : IDocumentPO
    {
        bool IsExpanded { get; }
        IMvxCommand ExpandOrCollapseCommand { get; }
        IMvxCommand SeeReviewCommand { get; }
        IMvxAsyncCommand OpenTopSongsCommand { get; }
        IBmmInteraction ExpandOrCollapseInteraction { get; }
    }
}