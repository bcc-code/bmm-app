using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using Android.Widget;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.YearInReview.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using FFImageLoading;
using FFImageLoading.Drawables;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Actions
{
    public class ShareYearInReviewAction : GuardedActionWithParameter<string>, IShareYearInReviewAction
    {
        private const string PngMimeType = "image/png";
        private readonly IMvxAndroidCurrentTopActivity _mvxAndroidCurrentTopActivity;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public ShareYearInReviewAction(
            IMvxAndroidCurrentTopActivity mvxAndroidCurrentTopActivity,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _mvxAndroidCurrentTopActivity = mvxAndroidCurrentTopActivity;
            _bmmLanguageBinder = bmmLanguageBinder;
        }
        
        protected override async Task Execute(string imageUrl)
        {
            string title = _bmmLanguageBinder[Translations.YearInReviewViewModel_ShareTitle];
            var imageView = new ImageView(_mvxAndroidCurrentTopActivity.Activity);

            await ImageService.Instance
                .LoadUrl(imageUrl)
                .IntoAsync(imageView);

            ContentValues values = new ContentValues();
            values.Put(MediaStore.Images.Media.InterfaceConsts.Title, title);
            values.Put(MediaStore.Images.Media.InterfaceConsts.MimeType, PngMimeType);
            var uri = _mvxAndroidCurrentTopActivity.Activity.ContentResolver!.Insert(MediaStore.Images.Media.ExternalContentUri!, values);

            var bitmap = ((FFBitmapDrawable)imageView.Drawable)!.Bitmap;
            var outStream = _mvxAndroidCurrentTopActivity.Activity.ContentResolver.OpenOutputStream(uri!);
            await bitmap!.CompressAsync(Bitmap.CompressFormat.Png, 100, outStream);
            outStream!.Close();
            
            var intent = new Intent(Intent.ActionSend);
            intent.SetType(PngMimeType);
            intent.PutExtra(Intent.ExtraStream, uri);
            intent.SetFlags(ActivityFlags.GrantReadUriPermission);
            _mvxAndroidCurrentTopActivity.Activity.StartActivity(Intent.CreateChooser(intent, title));
        }
    }
}