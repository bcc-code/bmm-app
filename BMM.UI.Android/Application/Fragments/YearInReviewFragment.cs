using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Fragments.Base;
using BMM.UI.Droid.Application.Listeners;
using BMM.UI.Droid.Application.Listeners.Interfaces;
using FFImageLoading.Extensions;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = true, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.YearInReviewFragment")]
    public class YearInReviewFragment : BaseDialogFragment<YearInReviewViewModel>, IRecyclerViewSnapHandler
    {
        private const int HeaderHeight = 56;
        private const int BottomImageMargin = 32;
        private const float ImageWidthToHeightRatio = 0.77f;
        private const float ImageHeightToViewHeightRatio = 0.6f;
        protected override int FragmentId => Resource.Layout.fragment_year_in_review;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var metrics = new DisplayMetrics();
            ((Activity)Context!).Display!.GetRealMetrics(metrics);
            
            int screenHeight = metrics.HeightPixels;
            int screenWidth = metrics.WidthPixels;
            
            int horizontalMargin = Resources.GetDimensionPixelSize(Resource.Dimension.margin_large) * 2;
            float imageHeight = (screenHeight - HeaderHeight.DpToPixels()) * ImageHeightToViewHeightRatio;
            float itemHeight = imageHeight + BottomImageMargin.DpToPixels();
            int itemWidth = screenWidth - horizontalMargin;
            double imageWidth = Math.Min(imageHeight * ImageWidthToHeightRatio, itemWidth);
            
            var adapter = new YearInReviewRecyclerAdapter(
                (IMvxAndroidBindingContext)BindingContext,
                itemWidth,
                itemHeight,
                imageWidth - Resources.GetDimensionPixelSize(Resource.Dimension.margin_small) * 2,
                imageHeight);
            
            var yearInReviewRecyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.YearInReviewRecyclerView);
            yearInReviewRecyclerView!.Adapter = adapter;
            yearInReviewRecyclerView!.UpdateHeight((int)itemHeight);
            
            var pagerSnapHelper = new PagerSnapHelper();
            pagerSnapHelper.AttachToRecyclerView(yearInReviewRecyclerView);
            yearInReviewRecyclerView.AddOnScrollListener(new SnapOnScrollListener(pagerSnapHelper, this));
            
            return view;
        }

        public void OnPositionChanged(int currentPosition)
        {
            ViewModel.CurrentPosition = currentPosition;
        }
    }
}