using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.DeepLinks.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Models.POs.YearInReview;
using BMM.Core.Models.POs.YearInReview.Interfaces;
using MvvmCross.Navigation;

namespace BMM.Core.Implementations.Factories.YearInReview
{
    public class YearInReviewTeaserPOFactory : IYearInReviewTeaserPOFactory
    {
        private readonly IInternalDeepLinkAction _internalDeepLinkAction;
        private readonly IMvxNavigationService _mvxNavigationService;
        private readonly IAnalytics _analytics;

        public YearInReviewTeaserPOFactory(
            IInternalDeepLinkAction internalDeepLinkAction,
            IMvxNavigationService mvxNavigationService,
            IAnalytics analytics)
        {
            _internalDeepLinkAction = internalDeepLinkAction;
            _mvxNavigationService = mvxNavigationService;
            _analytics = analytics;
        }
        
        public IYearInReviewTeaserPO Create(YearInReviewTeaser yearInReviewTeaser)
        {
            return new YearInReviewTeaserPO(yearInReviewTeaser, _internalDeepLinkAction, _mvxNavigationService, _analytics);
        }
    }
}