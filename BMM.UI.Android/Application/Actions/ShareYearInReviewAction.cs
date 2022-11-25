using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Acr.UserDialogs.Builders;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using Android.Widget;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.YearInReview.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using FFImageLoading;
using FFImageLoading.Drawables;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Android;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace BMM.UI.Droid.Application.Actions
{
    public class ShareYearInReviewAction : GuardedActionWithParameter<string>, IShareYearInReviewAction
    {
        private const string PngMimeType = "image/png";
        private readonly IMvxAndroidCurrentTopActivity _mvxAndroidCurrentTopActivity;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly IMvxNavigationService _mvxNavigationService;
        private readonly IToastDisplayer _toastDisplayer;

        public ShareYearInReviewAction(
            IMvxAndroidCurrentTopActivity mvxAndroidCurrentTopActivity,
            IBMMLanguageBinder bmmLanguageBinder,
            IMvxNavigationService mvxNavigationService,
            IToastDisplayer toastDisplayer)
        {
            _mvxAndroidCurrentTopActivity = mvxAndroidCurrentTopActivity;
            _bmmLanguageBinder = bmmLanguageBinder;
            _mvxNavigationService = mvxNavigationService;
            _toastDisplayer = toastDisplayer;
        }
        
        protected override async Task Execute(string imageUrl)
        {
            string title = _bmmLanguageBinder[Translations.YearInReviewViewModel_ShareTitle];
            var imageView = new ImageView(_mvxAndroidCurrentTopActivity.Activity);

            await ImageService.Instance
                .LoadUrl(imageUrl)
                .IntoAsync(imageView);
            
            var bitmap = ((FFBitmapDrawable)imageView.Drawable)!.Bitmap;

            var optionsList = new List<StandardIconOptionPO>
            {
                CreateSaveToGalleryItem(bitmap),
                GetShareExternallyItem(bitmap, title)
            };

            await _mvxNavigationService.Navigate<OptionsListViewModel, IOptionsListParameter>(new OptionsListParameter(optionsList));
        }

        private StandardIconOptionPO GetShareExternallyItem(Bitmap bitmap, string title)
        {
            return new StandardIconOptionPO(
                _bmmLanguageBinder[Translations.YearInReviewViewModel_ShareExternally],
                ImageResourceNames.IconShare,
                new ExceptionHandlingCommand(async () =>
                {
                    var values = new ContentValues();
                    values.Put(MediaStore.Images.Media.InterfaceConsts.Title, title);
                    values.Put(MediaStore.Images.Media.InterfaceConsts.MimeType, PngMimeType);
            
                    var uri = _mvxAndroidCurrentTopActivity.Activity.ContentResolver!.Insert(MediaStore.Images.Media.ExternalContentUri!, values);
                    
                    var outStream = _mvxAndroidCurrentTopActivity.Activity.ContentResolver!.OpenOutputStream(uri!);
                    await bitmap!.CompressAsync(Bitmap.CompressFormat.Png, 100, outStream);
                    outStream!.Close();
                    
                    var intent = new Intent(Intent.ActionSend);
                    intent.SetType(PngMimeType);
                    intent.PutExtra(Intent.ExtraStream, uri);
                    intent.SetFlags(ActivityFlags.GrantReadUriPermission);

                    _mvxAndroidCurrentTopActivity.Activity.StartActivity(Intent.CreateChooser(intent, title));
                }));
        }

        private StandardIconOptionPO CreateSaveToGalleryItem(Bitmap bitmap)
        {
            return new StandardIconOptionPO(
                _bmmLanguageBinder[Translations.YearInReviewViewModel_SaveToGallery],
                ImageResourceNames.IconGallery,
                new ExceptionHandlingCommand(async () =>
                {
                    string path = System.IO.Path.Combine(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures)!.AbsolutePath, $"year_in_review_{DateTime.Now.ToUnixTimestamp()}.png");
                    var fileStream = new FileStream(path, FileMode.OpenOrCreate);
                    await bitmap!.CompressAsync(Bitmap.CompressFormat.Png, 100, fileStream);
                    fileStream.Close();
                    await _toastDisplayer.Success(_bmmLanguageBinder[Translations.YearInReviewViewModel_SavedToGallery]);
                }));
        }
    }
}