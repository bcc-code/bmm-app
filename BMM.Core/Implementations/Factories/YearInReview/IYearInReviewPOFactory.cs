using BMM.Core.Models.POs.YearInReview.Interfaces;

namespace BMM.Core.Implementations.Factories.YearInReview
{
    public interface IYearInReviewPOFactory
    {
        IYearInReviewItemPO Create(
            string url,
            string subtitle,
            string description,
            string color);
    }
}