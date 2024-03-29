using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.YearInReview.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Models.POs.YearInReview.Interfaces;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class YearInReviewViewModel : BaseViewModel, IYearInReviewViewModel
    {
        public const int BottomImageMargin = 30;
        public const float ImageWidthToHeightRatio = 0.7f;
        public const float ImageHeightToViewHeightRatio = 0.6f;
        public const int HeaderHeight = 56;
        
        private readonly IInitializeYearInReviewViewModelAction _initializeYearInReviewViewModelAction;
        private int _currentPosition;
        private string _description;

        public YearInReviewViewModel(
            IInitializeYearInReviewViewModelAction initializeYearInReviewViewModelAction,
            IShareYearInReviewAction shareYearInReviewAction,
            IAnalytics analytics)
        {
            _initializeYearInReviewViewModelAction = initializeYearInReviewViewModelAction;
            _initializeYearInReviewViewModelAction.AttachDataContext(this);
            ShareCommand = new MvxAsyncCommand(() =>
            {
                analytics.LogEvent(Event.YearInReviewShareClicked);
                return shareYearInReviewAction.ExecuteGuarded(CurrentElement.Url);
            });
        }

        public IMvxAsyncCommand ShareCommand { get; }

        public string Description
        {
            get => _description;
            private set => SetProperty(ref _description, value);
        }

        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                
                if (!YearInReviewElements.Any())
                    return;
                
                Description = YearInReviewElements[_currentPosition].Description;
            }
        }

        public IYearInReviewItemPO CurrentElement => YearInReviewElements[_currentPosition];

        public IBmmObservableCollection<IYearInReviewItemPO> YearInReviewElements { get; } =
            new BmmObservableCollection<IYearInReviewItemPO>();

        public override async Task Initialize()
        {
            await base.Initialize();
            await _initializeYearInReviewViewModelAction.ExecuteGuarded();
        }
    }
}