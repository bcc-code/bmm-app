using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.YearInReview.Interfaces;
using BMM.Core.Implementations.Factories.YearInReview;
using BMM.Core.Models.POs.YearInReview.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.YearInReview
{
    public class InitializeYearInReviewViewModelAction : GuardedAction, IInitializeYearInReviewViewModelAction
    {
        private readonly IStatisticsClient _statisticsClient;
        private readonly IYearInReviewPOFactory _yearInReviewPOFactory;
        private readonly IShareYearInReviewAction _shareYearInReviewAction;

        public InitializeYearInReviewViewModelAction(
            IStatisticsClient statisticsClient,
            IYearInReviewPOFactory yearInReviewPOFactory,
            IShareYearInReviewAction shareYearInReviewAction)
        {
            _statisticsClient = statisticsClient;
            _yearInReviewPOFactory = yearInReviewPOFactory;
            _shareYearInReviewAction = shareYearInReviewAction;
        }
        
        private IYearInReviewViewModel DataContext => this.GetDataContext();

        protected override async Task Execute()
        {
            var yearInReviewList = await _statisticsClient.GetYearInReview();
            var yearInReviewItemPOs = new List<IYearInReviewItemPO>();

            foreach (var yearInReviewItem in yearInReviewList)
            {
                yearInReviewItemPOs.Add(_yearInReviewPOFactory.Create(
                    yearInReviewItem.Url,
                    yearInReviewItem.Subtitle,
                    yearInReviewItem.Description,
                    yearInReviewItem.Color));
            }
            
            DataContext.YearInReviewElements.ReplaceWith(yearInReviewItemPOs);
            DataContext.CurrentPosition = 0;
        }
    }
}