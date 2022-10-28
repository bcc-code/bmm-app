using BMM.Core.Models.POs.YearInReview;
using BMM.Core.Models.POs.YearInReview.Interfaces;

namespace BMM.Core.Implementations.Factories.YearInReview
{
    public class YearInReviewPOFactory : IYearInReviewPOFactory
    {
        public IYearInReviewItemPO Create(string url, string subtitle, string description, string color)
        {
            return new YearInReviewItemPO(
                url,
                subtitle,
                description,
                color);
        }
    }
}