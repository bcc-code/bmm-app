using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.YearInReview.Interfaces
{
    public interface IYearInReviewItemPO : IBasePO
    {
        string Url { get; }
        string Subtitle { get; }
        string Description { get; }
        string Color { get; }
    }
}