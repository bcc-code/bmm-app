using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.YearInReview.Interfaces;

namespace BMM.Core.Implementations.Factories.YearInReview
{
    public interface IYearInReviewTeaserPOFactory
    {
        IYearInReviewTeaserPO Create(YearInReviewTeaser yearInReviewTeaser);
    }
}