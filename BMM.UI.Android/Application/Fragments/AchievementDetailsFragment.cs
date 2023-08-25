using Android.Runtime;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Fragments.Base;
using BMM.UI.Droid.Application.ViewHolders;
using Com.Github.Jinatonic.Confetti;
using FFImageLoading.Cross;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.AchievementDetailsFragment")]
    public class AchievementDetailsFragment : BaseDialogFragment<AchievementDetailsViewModel>
    {
        private bool _shouldShowConfetti;
        private bool _confettiShown;
        private string _imagePath;
        private MvxCachedImageView _imageView;
        protected override int FragmentId => Resource.Layout.fragment_achievements_details;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            _imageView = view.FindViewById<MvxCachedImageView>(Resource.Id.AchievementImage);
            var set = this.CreateBindingSet<AchievementDetailsFragment, AchievementDetailsViewModel>();
            
            set.Bind(this)
                .For(s => s.ShouldShowConfetti)
                .To(vm => vm.ShouldShowConfetti);
        
            set.Bind(this)
                .For(v => v.ImagePath)
                .To(po => po.AchievementPO.ImagePath);

            set.Apply();
            return view;
        }
        
        public bool ShouldShowConfetti
        {
            get => _shouldShowConfetti;
            set
            {
                _shouldShowConfetti = value;

                if (_shouldShowConfetti && !_confettiShown)
                {
                    _confettiShown = true;
                    ShowConfetti();
                }
            }
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                
                if (_imagePath.IsNullOrEmpty())
                {
                    _imageView.ImagePath  = Context.IsNightMode()
                        ? AchievementViewHolder.NightModeIcon
                        : AchievementViewHolder.StandardModeIcon;
                }
            }
        }

        private void ShowConfetti()
        {
            View!.Post(() =>
            {
                var parent = View!.FindViewById<ConstraintLayout>(Resource.Id.ParentView);
                CommonConfetti.RainingConfetti(parent, 
                    new int[]
                    {
                        Context.GetColorFromResource(Resource.Color.bible_study_confetti_one_color),
                        Context.GetColorFromResource(Resource.Color.bible_study_confetti_two_color),
                        Context.GetColorFromResource(Resource.Color.bible_study_confetti_three_color),
                    })!.Infinite().SetVelocityY(350)!.SetEmissionDuration(5000);
            });
        }
    }
}