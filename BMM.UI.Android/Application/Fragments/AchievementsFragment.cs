using Android.Flexbox;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.Other;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.ItemDecorators;
using FFImageLoading.Extensions;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.AchievementsFragment")]
    public class AchievementsFragment : BaseFragment<AchievementsViewModel>
    {
        private const int MarginBetweenItems = 16;

        protected override int FragmentId => Resource.Layout.fragment_achievements;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view =  base.OnCreateView(inflater, container, savedInstanceState);

            var achievementsRecyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.AchievementsRecyclerView);

            view!.Post(() =>
            {
                var layoutManager = new FlexboxLayoutManager(Context, IFlexDirection.Column);
                layoutManager.FlexWrap = IFlexWrap.Wrap;
                layoutManager.FlexDirection = IFlexDirection.Row;
                layoutManager.AlignItems = IAlignItems.FlexStart;
                
                achievementsRecyclerView!.AddItemDecoration(new ProfileAchievementsItemDecorator(MarginBetweenItems.DpToPixels()));
                achievementsRecyclerView.SetLayoutManager(layoutManager);
            });
            
            return view;
        }
    }
}