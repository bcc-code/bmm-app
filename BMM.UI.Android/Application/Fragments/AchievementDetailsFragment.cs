using Android.Runtime;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Fragments.Base;
using Com.Github.Jinatonic.Confetti;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.AchievementDetailsFragment")]
    public class AchievementDetailsFragment : BaseDialogFragment<AchievementDetailsViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_achievement_fragment;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            view.Post(() =>
            {
                var parent = view.FindViewById<ConstraintLayout>(Resource.Id.ParentView);
                CommonConfetti.RainingConfetti(parent, 
                    new int[]
                    {
                        Context.GetColorFromResource(Resource.Color.bible_study_confetti_one_color),
                        Context.GetColorFromResource(Resource.Color.bible_study_confetti_two_color),
                        Context.GetColorFromResource(Resource.Color.bible_study_confetti_three_color),
                    })!.Infinite()!.SetEmissionDuration(5000);
            });
            
            return view;
        }
    }
}