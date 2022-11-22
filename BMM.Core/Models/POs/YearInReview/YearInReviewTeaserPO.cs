using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.DeepLinks.Interfaces;
using BMM.Core.GuardedActions.DeepLinks.Parameters;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Storage;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.YearInReview.Interfaces;
using BMM.Core.Models.Storage.Persistent;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.YearInReview
{
    public class YearInReviewTeaserPO 
        : DocumentPO,
          IYearInReviewTeaserPO
    {
        private const string Origin = "Year in Review Preview";
        
        public YearInReviewTeaserPO(
            YearInReviewTeaser yearInReviewTeaser,
            IInternalDeepLinkAction internalDeepLinkAction,
            IMvxNavigationService mvxNavigationService,
            IAnalytics analytics) : base(yearInReviewTeaser)
        {
            YearInReviewTeaser = yearInReviewTeaser;
            ExpandOrCollapseCommand = new MvxCommand(() =>
            {
                IsExpanded = !IsExpanded;
                AppLifetimeMemory.YearInReviewTeaserExpanded = IsExpanded;
                ExpandOrCollapseInteraction?.Raise();
            });
            SeeReviewCommand = new MvxCommand(() =>
            {
                internalDeepLinkAction.ExecuteGuarded(new InternalDeepLinkActionParameter(yearInReviewTeaser.ButtonLink, Origin));
            });
            OpenTopSongsCommand = new MvxAsyncCommand(async () =>
            {
                analytics.LogEvent(Event.YearInReviewPlaylistOpened);
                await mvxNavigationService.Navigate<TopSongsCollectionViewModel>();
            });
            ExpandOrCollapseInteraction = new BmmInteraction();

            ExpandTeaserIfNeeded();
        }

        private void ExpandTeaserIfNeeded()
        {
            if (!AppSettings.YearInReviewShown)
            {
                AppSettings.YearInReviewShown = true;
                AppLifetimeMemory.YearInReviewTeaserExpanded = true;
            }

            IsExpanded = AppLifetimeMemory.YearInReviewTeaserExpanded;
        }

        public YearInReviewTeaser YearInReviewTeaser { get; }
        public bool IsExpanded { get; private set; }
        public IMvxCommand ExpandOrCollapseCommand { get; }
        public IMvxCommand SeeReviewCommand { get; }
        public IMvxAsyncCommand OpenTopSongsCommand { get; }
        public IBmmInteraction ExpandOrCollapseInteraction { get; }
    }
}