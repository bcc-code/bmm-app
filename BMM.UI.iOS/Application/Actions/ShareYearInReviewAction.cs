using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.YearInReview.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using CoreGraphics;
using FFImageLoading;
using Foundation;
using LinkPresentation;
using MvvmCross.Base;
using UIKit;

namespace BMM.UI.iOS.Actions
{
    public class ShareYearInReviewAction 
        : GuardedActionWithParameter<string>,
          IShareYearInReviewAction
    {
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadDispatcher;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public ShareYearInReviewAction(
            IMvxMainThreadAsyncDispatcher mvxMainThreadDispatcher,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _mvxMainThreadDispatcher = mvxMainThreadDispatcher;
            _bmmLanguageBinder = bmmLanguageBinder;
        }

        protected override async Task Execute(string parameter)
        {
            await _mvxMainThreadDispatcher.ExecuteOnMainThreadAsync(async () =>
            {
                var imageView = new UIImageView();
                await ImageService.Instance.LoadUrl(parameter).IntoAsync(imageView);
                
                var shareViewController = new UIActivityViewController(new NSObject[] { new ShareActivityItemSource(imageView.Image,  _bmmLanguageBinder[Translations.YearInReviewViewModel_ShareTitle]) }, null);
                
                if (shareViewController.PopoverPresentationController != null)
                {
                    var sourceView = YearInReviewViewController.Instance!.ButtonShare;
                    shareViewController.PopoverPresentationController.SourceView = sourceView;
                }
                
                YearInReviewViewController.Instance!.PresentViewController(shareViewController, true, null);
            });
        }

        class ShareActivityItemSource : UIActivityItemSource
        {
            private readonly UIImage _item;
            private readonly string _subject;

            internal ShareActivityItemSource(
                UIImage item,
                string subject)
            {
                _item = item;
                _subject = subject;
            }

            public override LPLinkMetadata GetLinkMetadata(UIActivityViewController activityViewController)
            {
                var meta =  new LPLinkMetadata();
                meta.Title = _subject;
                meta.IconProvider = new NSItemProvider(_item);
                return meta;
            }

            public override NSObject GetItemForActivity(UIActivityViewController activityViewController, NSString activityType) => _item;

            public override NSObject GetPlaceholderData(UIActivityViewController activityViewController) => _item;

            public override UIImage GetThumbnailImageForActivity(UIActivityViewController activityViewController, NSString activityType, CGSize suggestedSize) => _item;

            public override string GetSubjectForActivity(UIActivityViewController activityViewController, NSString activityType) => _subject;
        }
    }
}