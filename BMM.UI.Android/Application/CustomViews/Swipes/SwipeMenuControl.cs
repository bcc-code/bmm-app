using Android.Animation;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using BMM.Core.Models.Enums;
using BMM.UI.Droid.Application.ViewHolders.Base;
using LayoutDirection = Android.Views.LayoutDirection;

namespace BMM.UI.Droid.Application.CustomViews.Swipes
{
    public class SwipeMenuControl : LinearLayout, ValueAnimator.IAnimatorUpdateListener
    {
        public const int MinAnimationThreshold = 15;

        private ValueAnimator _animator;
        private readonly IList<SwipeMenuView> _items = new List<SwipeMenuView>();
        private SwipeMenuViewHolder _viewHolder;

        public SwipeMenuControl(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected SwipeMenuControl(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SwipeMenuControl(int itemWidth, SwipePlacement placement, Context context, IAttributeSet attrs) : base(
            context,
            attrs)
        {
            ItemWidth = itemWidth;
            Orientation = Orientation.Horizontal;
            Placement = placement;

            if (Placement == SwipePlacement.Right)
            {
                LayoutParameters = new FrameLayout.LayoutParams(
                    LayoutParams.MatchParent,
                    LayoutParams.MatchParent,
                    GravityFlags.Right);
                LayoutDirection = LayoutDirection.Rtl;
            }
        }

        public int AnimatedChildIndex { get; set; } = 0;
        public bool IsOpen { get; set; }
        public SwipePlacement Placement { get; set; }
        public int ItemWidth { get; set; }
        public int ItemWidthInPx => (int)(DpFactor * ItemWidth);
        public float DpFactor => Resources.DisplayMetrics.Density;
        public int FullSizeWidth => ItemWidthInPx * _items.Count(x => x.Available);
        public int EdgeSnappingPoint => FullSizeWidth > ItemWidthInPx ? FullSizeWidth / 2 : (int)(ItemWidthInPx * 0.8f);
        public int EdgeClosingPoint => FullSizeWidth - EdgeSnappingPoint;
        public int FullSwipeSnappingPoint => (int)((Parent as ViewGroup).Width * 0.7f);
        public bool AllowFullSwipe { get; set; } = true;

        public bool FullSwipeAvailable
            => AllowFullSwipe && (_items.FirstOrDefault(x => x.Available)?.CanExecuteFullSwipe ?? false);

        public void AddSwipeMenuItem(SwipeMenuView item)
        {
            _items.Add(item);

            var layoutParams = new LayoutParams(ItemWidthInPx, LayoutParams.MatchParent);
            if (ChildCount == 0)
            {
                item.SetMinimumWidth(ItemWidthInPx);
            }

            item.AddShadowIfNeeded(Placement, ItemWidthInPx);
            item.AddSeparatorIfNeeded();
            item.LayoutParameters = layoutParams;
            AddView(item);
        }

        public void ExecuteFullSwipe(object dataContext)
        {
            if (!(_items.FirstOrDefault(x => x.Available) is { } firstItem))
                return;

            var command = firstItem.FullSwipeCommand ?? firstItem.ClickCommand;
            if (command?.CanExecute(dataContext) ?? false)
                command.Execute(dataContext);
        }

        public void RefreshCanExecuteForAllChildren()
        {
            foreach (var item in _items)
                item.RefreshEnabledState();
        }

        public int ResizeWidth(int width, bool animate = false)
        {
            if (!FullSwipeAvailable)
                width = Math.Min(width, FullSizeWidth);

            if (Width == 0)
                IsOpen = false;
            else if (Width >= FullSizeWidth)
                IsOpen = true;

            if (!animate || Math.Abs(Width - width) < MinAnimationThreshold)
            {
                CancelAnimatorIfNeeded();
                SetWidth(width);
                return width;
            }

            CancelAnimatorIfNeeded();
            _animator = ValueAnimator.OfInt(Width, width);
            _animator.AddUpdateListener(this);
            _animator.AnimationEnd += AnimatorOnAnimationEnd;
            _animator.Start();
            return width;
        }

        private void AnimatorOnAnimationEnd(object sender, EventArgs e)
        {
            _animator = null;
        }

        private void CancelAnimatorIfNeeded()
        {
            if (_animator != null)
            {
                _animator.Cancel();
                _animator.AnimationEnd -= AnimatorOnAnimationEnd;
                _animator.RemoveAllUpdateListeners();
                _animator = null;
            }
        }

        private void SetWidth(int width)
        {
            var layoutParams = LayoutParameters;
            layoutParams.Width = width;
            LayoutParameters = layoutParams;

            var childLayoutPrams = (LinearLayout.LayoutParams)GetChildAt(AnimatedChildIndex)?.LayoutParameters;

            if (childLayoutPrams == null)
                return;

            if (width > FullSizeWidth)
            {
                childLayoutPrams.Weight = 1;
            }
            else
            {
                childLayoutPrams.Weight = 0;
            }

            if (width > FullSwipeSnappingPoint)
            {
                HideAllChildrenExceptFirstOne();
            }
            else
            {
                MakeSureAllChildrenVisible();
            }

            GetChildAt(AnimatedChildIndex).LayoutParameters = childLayoutPrams;
        }

        public void MakeSureAllChildrenVisible()
        {
            SetChildrenVisibility(ViewStates.Visible);

            SwipeMenuView firstItem = null;
            if (_items.Any() && (firstItem = _items.FirstOrDefault(x => x.Available)) != null)
            {
                AnimatedChildIndex = _items.IndexOf(firstItem);
            }
        }

        private void SetChildrenVisibility(ViewStates viewState)
        {
            for (int i = 0; i < ChildCount; i++)
            {
                var menuItem = GetChildAt(i) as SwipeMenuView;
                menuItem.Visibility = menuItem.Available ? viewState : ViewStates.Gone;
            }
        }

        public void HideAllChildrenExceptFirstOne()
        {
            SetChildrenVisibility(ViewStates.Gone);

            if (ChildCount > 0)
            {
                var menuItem = _items.FirstOrDefault(x => x.Available);
                menuItem.Visibility = ViewStates.Visible;
            }
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            SetWidth((int)animation.AnimatedValue);
        }

        public void SetViewHolder(SwipeMenuViewHolder viewHolder) => _viewHolder = viewHolder;
        public void RequestHideMenu() => _viewHolder?.HideMenusIfNeeded();
        public void RequestShowMenu(SwipePlacement placement) => _viewHolder?.ShowMenu(placement);
    }
}