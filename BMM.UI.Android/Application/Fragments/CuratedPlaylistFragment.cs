using System.ComponentModel;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Helpers;
using FFImageLoading;
using Google.Android.Material.AppBar;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.CuratedPlaylistFragment")]
    public class CuratedPlaylistFragment : BaseFragment<CuratedPlaylistViewModel>
    {
        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
        }

        public override void OnDestroy()
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            base.OnDestroy();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var propertyName = args.PropertyName;
            var allPropertiesHaveChanged = string.IsNullOrEmpty(propertyName);

            if (allPropertiesHaveChanged || args.PropertyName == "CuratedPlaylist")
            {
                if (ViewModel.CuratedPlaylist?.Cover == null)
                    return;

                Mvx.IoCProvider.Resolve<IExceptionHandler>()
                    .FireAndForgetWithoutUserMessages(async () =>
                    {
                        var drawable = await ImageService.Instance.LoadUrl(ViewModel.CuratedPlaylist.Cover).AsBitmapDrawableAsync();
                        var bitmap = drawable.Bitmap;
                        var newStatusbarColor = new Color(BitmapHelper.GetColor(bitmap));
                        FragmentBaseColor = newStatusbarColor;

                        await Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>()
                            .ExecuteOnMainThreadAsync(() => { SetStatusBarColor(newStatusbarColor); });
                    });
            }
        }

        protected override int FragmentId => Resource.Layout.fragment_tracklist;
    }
}