using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.DeepLinks.Interfaces;
using BMM.Core.Models.POs.YearInReview;
using BMM.Core.Models.POs.YearInReview.Interfaces;

namespace BMM.Core.Implementations.Factories.YearInReview
{
    public class YearInReviewPreviewPOFactory : IYearInReviewPreviewPOFactory
    {
        private readonly IInternalDeepLinkAction _internalDeepLinkAction;

        public YearInReviewPreviewPOFactory(IInternalDeepLinkAction internalDeepLinkAction)
        {
            _internalDeepLinkAction = internalDeepLinkAction;
        }
        
        public IYearInReviewPreviewPO Create(YearInReviewPreview yearInReviewPreview)
        {
            return new YearInReviewPreviewPO(yearInReviewPreview, _internalDeepLinkAction);
        }
    }
}