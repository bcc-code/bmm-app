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

    public class SnapOnScrollListener : RecyclerView.OnScrollListener
    {
        private readonly SnapHelper _snapHelper;
        private readonly IRecyclerViewSnapHandler _recyclerViewSnapHandler;
        private int _snapPosition = RecyclerView.NoPosition;

        public SnapOnScrollListener(SnapHelper snapHelper, IRecyclerViewSnapHandler recyclerViewSnapHandler)
        {
            _snapHelper = snapHelper;
            _recyclerViewSnapHandler = recyclerViewSnapHandler;
        }
        
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            NotifySnapPositionChangeIfNeeded(recyclerView);
        }

        private void NotifySnapPositionChangeIfNeeded(RecyclerView recyclerView)
        {
            int snapPosition = GetSnapPosition(_snapHelper, recyclerView);
            bool snapPositionChanged = snapPosition != _snapPosition;
            if (snapPositionChanged)
            {
                _snapPosition = snapPosition;
                _recyclerViewSnapHandler?.OnPositionChanged(_snapPosition);
            }
        }

        private int GetSnapPosition(SnapHelper snapHelper, RecyclerView recyclerView)
        {
            var layoutManager = recyclerView.GetLayoutManager();
            var snapView = snapHelper.FindSnapView(layoutManager);
            return layoutManager!.GetPosition(snapView!);
        }
    }

    public interface IRecyclerViewSnapHandler
    {
        void OnPositionChanged(int currentPosition);
    }
}