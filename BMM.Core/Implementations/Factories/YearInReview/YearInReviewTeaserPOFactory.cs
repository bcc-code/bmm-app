using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.DeepLinks.Interfaces;
using BMM.Core.Models.POs.YearInReview;
using BMM.Core.Models.POs.YearInReview.Interfaces;

namespace BMM.Core.Implementations.Factories.YearInReview
{
    public class YearInReviewTeaserPOFactory : IYearInReviewTeaserPOFactory
    {
        private readonly IInternalDeepLinkAction _internalDeepLinkAction;

        public YearInReviewTeaserPOFactory(IInternalDeepLinkAction internalDeepLinkAction)
        {
            _internalDeepLinkAction = internalDeepLinkAction;
        }
        
        public IYearInReviewTeaserPO Create(YearInReviewTeaser yearInReviewTeaser)
        {
            return new YearInReviewTeaserPO(yearInReviewTeaser, _internalDeepLinkAction);
        }
    }
}