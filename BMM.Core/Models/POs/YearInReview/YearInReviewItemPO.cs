using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.YearInReview.Interfaces;

namespace BMM.Core.Models.POs.YearInReview
{
    public class YearInReviewItemPO : BasePO, IYearInReviewItemPO
    {
        public YearInReviewItemPO(
            string url,
            string subtitle,
            string description,
            string color)
        {
            Url = url;
            Subtitle = subtitle;
            Description = description;
            Color = color;
        }

        public string Url { get; }
        public string Subtitle { get; }
        public string Description { get; }
        public string Color { get; }
    }
}