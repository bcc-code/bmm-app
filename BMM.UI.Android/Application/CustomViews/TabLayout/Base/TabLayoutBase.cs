using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross.Binding.Extensions;
using MvvmCross.Commands;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.WeakSubscription;

namespace BMM.UI.Droid.Application.CustomViews.TabLayout.Base
{
    public abstract class TabLayoutBase : FrameLayout, INotifyPropertyChanged, ViewPager.IOnPageChangeListener
    {
        private const int InvalidIndex = -1;

        private MvxRecyclerView _recyclerView;
        private FixedLengthIndicator _indicator;
        private CenterSnapHelper _scrollHelper;
        private ViewPager _viewPager;
        private MvxNotifyCollectionChangedEventSubscription _itemsSubscription;
        private bool _firstLayout;
        private SearchResultsViewModel _selectedItem;

        public event EventHandler<int> OnTabChanged;

        protected TabLayoutBase(IntPtr javaReference, JniHandleOwnership transfer) : base(
            javaReference,
            transfer)
        {
        }

        public TabLayoutBase(Context context) : base(context)
        {
            Initialize(context);
        }

        public TabLayoutBase(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs);
        }

        protected virtual void InitializeAttributes(Context context, IAttributeSet attrs) { }
        protected abstract BaseTabLayoutAdapter Adapter { get; }
        protected abstract int LayoutId { get; }
        protected virtual RecyclerView.ItemDecoration ItemDecorator { get; }
        protected virtual int ItemSpacing { get; }

        public SearchResultsViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetSelectedItem(value);
        }

        public ViewPager ViewPager
        {
            get => _viewPager;
            set => _viewPager = value;
        }

        private void Initialize(Context context, IAttributeSet attrs = null)
        {
            if(attrs != null)
                InitializeAttributes(context, attrs);

            var inflater = new MvxLayoutInflater(context);
            inflater.Inflate(LayoutId, this);

            _recyclerView = FindViewById<MvxRecyclerView>(Resource.Id.RecyclerTabLayout);
            _indicator = FindViewById<FixedLengthIndicator>(Resource.Id.FixedLengthIndicator);

            _recyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false));
            _recyclerView.Adapter = Adapter;
            _recyclerView.SetForegroundGravity(GravityFlags.CenterHorizontal);
            _recyclerView.ScrollChange += RecyclerViewOnScrollChange;

            if (ItemDecorator != null)
                _recyclerView.AddItemDecoration(ItemDecorator);

            _scrollHelper = new CenterSnapHelper();
            _scrollHelper.AttachToRecyclerView(_recyclerView);
        }

        public int SelectedIndex
            => SelectedItem != null ? ItemsSource?.GetPosition(SelectedItem) ?? InvalidIndex : InvalidIndex;

        private void RecyclerViewOnScrollChange(object sender, ScrollChangeEventArgs e)
        {
            EnsureSelectedItemHasUnderline();
        }

        private void EnsureSelectedItemHasUnderline()
        {
            var selectedView = _recyclerView.FindViewHolderForAdapterPosition(SelectedIndex);

            int centerX = 0;
            int width = 0;

            if (selectedView != null)
            {
                centerX = (int)selectedView.ItemView.GetX() + selectedView.ItemView.Width / 2;
                width = selectedView.ItemView.Width;
            }
            else
            {
                //offscreen
                var first = _recyclerView.GetChildAdapterPosition(_recyclerView.GetChildAt(0));
                var last = _recyclerView.GetChildAdapterPosition(
                    _recyclerView.GetChildAt(_recyclerView.ChildCount - 1));

                if (SelectedIndex == InvalidIndex)
                {
                    centerX = _indicator.LastPosition;
                }
                else if (SelectedIndex < first)
                    centerX = -(int)_indicator.IndicatorWidth * 2;
                else if (SelectedIndex > last)
                    centerX = Width + (int)_indicator.IndicatorWidth * 2;
            }

            _indicator.SetTo(centerX, width + ItemSpacing * 2);
        }

        public IEnumerable ItemsSource
        {
            get { return Adapter.ItemsSource; }
            set
            {
                _itemsSubscription?.Dispose();
                _itemsSubscription = null;
                Adapter.ItemsSource = value;
                Adapter.ItemClick = new MvxCommand<SearchResultsViewModel>(ItemClickAction);
                if (Adapter.ItemsSource is INotifyCollectionChanged newObservable)
                    _itemsSubscription = newObservable.WeakSubscribe(OnItemsSourceCollectionChanged);
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _recyclerView.SetAdapter(Adapter);
        }

        private void ItemClickAction(SearchResultsViewModel obj)
        {
            SelectedItem = obj;
            OnTabChanged?.Invoke(this, SelectedIndex);
        }

        private void ScrollToIndexWithIndicator(int selectedIndex)
        {
            var layoutManager = _recyclerView.GetLayoutManager();

            var viewStart = layoutManager.GetChildAt(0);
            var viewEnd = layoutManager.GetChildAt(layoutManager.ChildCount - 1);

            int positionStart = _recyclerView.GetChildAdapterPosition(viewStart);
            int positionEnd = _recyclerView.GetChildAdapterPosition(viewEnd);

            if (selectedIndex == 0)
            {
                _recyclerView.SmoothScrollToPosition(selectedIndex);
                var view = layoutManager.GetChildAt(0);
                if (view != null)
                    _indicator.MoveTo(view.Width / 2 + (int)view.GetX(), view.Width + ItemSpacing * 2);
            }
            else if (selectedIndex == ItemsSource.Count() - 1)
            {
                _recyclerView.SmoothScrollToPosition(selectedIndex);
                var view = layoutManager.GetChildAt(_recyclerView.ChildCount - 1);
                if (view != null)
                    _indicator.MoveTo((int)view.GetX() + view.Width / 2, view.Width + ItemSpacing * 2);
            }
            else
            {
                if (positionStart <= selectedIndex && selectedIndex <= positionEnd)
                {
                    var viewHolder = _recyclerView.FindViewHolderForAdapterPosition(selectedIndex);
                    if (viewHolder != null)
                    {
                        _indicator.MoveTo(
                            (int)viewHolder.ItemView.GetX() + viewHolder.ItemView.Width / 2,
                            viewHolder.ItemView.Width + ItemSpacing * 2);
                        _scrollHelper.ScrollTo(selectedIndex);
                    }
                }
                else
                {
                    _recyclerView.SmoothScrollToPosition(selectedIndex);
                }
            }
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);

            if (!_firstLayout)
            {
                if (SelectedItem == null && ItemsSource.Count() > 0)
                    SelectedItem = (SearchResultsViewModel)ItemsSource.ElementAt(0);
                EnsureSelectedItemHasUnderline();
                _firstLayout = true;
            }
        }

        private void SetSelectedItem(SearchResultsViewModel value)
        {
            if (_selectedItem == value)
                return;

            _selectedItem = value;

            if (_selectedItem == null)
                return;
            
            ScrollToIndexWithIndicator(SelectedIndex);

            if (_viewPager.CurrentItem != SelectedIndex)
                _viewPager.SetCurrentItem(SelectedIndex, true);
        }

        public void OnPageScrollStateChanged(int state)
        {
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
        }

        public void OnPageSelected(int position)
        {
            SelectedItem = (SearchResultsViewModel)ItemsSource.ElementAt(position);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}