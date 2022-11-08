using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.DeepLinks.Interfaces;
using BMM.Core.GuardedActions.DeepLinks.Parameters;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.YearInReview.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.YearInReview
{
    public class YearInReviewTeaserPO 
        : DocumentPO,
          IYearInReviewTeaserPO
    {
        private const string Origin = "Year in Review Preview";
        
        public YearInReviewTeaserPO(
            YearInReviewTeaser yearInReviewTeaser,
            IInternalDeepLinkAction internalDeepLinkAction) : base(yearInReviewTeaser)
        {
            YearInReviewTeaser = yearInReviewTeaser;
            ExpandOrCollapseCommand = new MvxCommand(() =>
            {
                IsExpanded = !IsExpanded;
                ExpandOrCollapseInteraction?.Raise();
            });
            SeeReviewCommand = new MvxCommand(() =>
            {
                internalDeepLinkAction.ExecuteGuarded(new InternalDeepLinkActionParameter(yearInReviewTeaser.ButtonLink, Origin));
            });
            ExpandOrCollapseInteraction = new BmmInteraction();
        }
        
        public YearInReviewTeaser YearInReviewTeaser { get; }
        public bool IsExpanded { get; private set; }
        public IMvxCommand ExpandOrCollapseCommand { get; }
        public IMvxCommand SeeReviewCommand { get; }
        public IBmmInteraction ExpandOrCollapseInteraction { get; }
    }
}