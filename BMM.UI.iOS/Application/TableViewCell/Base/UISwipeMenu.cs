using BMM.Core.Models.Enums;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews.Swipes.Base;
using BMM.UI.iOS.Enums;
using BMM.UI.iOS.TableViewCell.Base;
using MvvmCross.Commands;

namespace BMM.UI.iOS.TableViewCell.Base
{
    public class UISwipeMenu : UIView
    {
        private readonly SwipeableViewCell _swipeableViewCell;
        private NSLayoutConstraint _smallViewConstraint;
        private NSLayoutConstraint _expandedViewConstraint;
        private NSLayoutConstraint _expandedWidthViewConstraint;
        private NSLayoutConstraint _heightConstraint;
        private const double EdgePercent = 0.7;

        private readonly IList<SwipeMenuBase> _items = new List<SwipeMenuBase>();

        public UISwipeMenu(SwipePlacement placement, SwipeableViewCell swipeableViewCell, float itemWidth = 80)
        {
            _swipeableViewCell = swipeableViewCell;
            ItemWidth = itemWidth;
            Placement = placement;
            TranslatesAutoresizingMaskIntoConstraints = false;
            ClipsToBounds = true;
        }

        public float MaxSwipePoint => (float)(FullSwipeAvailable
            ? Superview?.Frame.Width ?? 0 + (Subviews.Length - 1) * ItemWidth
            : Subviews.Length * ItemWidth);

        public float ItemWidth { get; set; }
        public SwipePlacement Placement { get; private set; }
        public bool AllowFullSwipe { get; set; } = true;

        public bool FullSwipeAvailable
            => AllowFullSwipe && (_items.FirstOrDefault(x => x.Available)?.CanExecuteFullSwipe ?? false);

        public async Task ExecuteFullSwipe(object dataContext)
        {
            if (!(_items.FirstOrDefault(x => x.Available) is { } firstItem))
                return;

            var command = firstItem.FullSwipeCommand ?? firstItem.ClickCommand;
            if (command?.CanExecute(dataContext) ?? false)
            {
                // TODO: Refactor all commands used in swipes to async with one base interface
                switch (command)
                {
                    case IMvxAsyncCommand asyncCommand:
                        await asyncCommand.ExecuteAsync(dataContext as IBasePO);
                        break;
                    case IMvxAsyncCommand<IBasePO> asyncCommandWithParam when dataContext is IBasePO basePO:
                        await asyncCommandWithParam.ExecuteAsync(basePO);
                        break;
                    default:
                        command.Execute(dataContext);
                        break;
                }
            }
        }

        public void RequestHideMenus() => _swipeableViewCell.AnimateResetSwipe();

        public override void LayoutSubviews()
        {
            if (FullSwipeAvailable && Frame.Width > _items.Count(x => x.Available) * ItemWidth)
            {
                if (_smallViewConstraint != null)
                    _smallViewConstraint.Active = false;

                if (_expandedViewConstraint != null)
                    _expandedViewConstraint.Active = true;

                if (_expandedWidthViewConstraint != null)
                    _expandedWidthViewConstraint.Active = true;
            }
            else
            {
                if (_smallViewConstraint != null)
                    _smallViewConstraint.Active = true;

                if (_expandedViewConstraint != null)
                    _expandedViewConstraint.Active = false;

                if (_expandedWidthViewConstraint != null)
                    _expandedWidthViewConstraint.Active = false;
            }

            base.LayoutSubviews();
        }

        public void UpdateAvailability()
        {
            var items = _items.ToArray();
            _items.Clear();

            foreach (var menuItem in items)
                menuItem.RemoveFromSuperview();

            foreach (var menuItem in items)
            {
                if (menuItem.Available)
                    AddItem(menuItem);
            }

            _heightConstraint.Constant = Superview.Frame.Height;
        }

        public override void MovedToSuperview()
        {
            base.MovedToSuperview();

            NSLayoutConstraint.Create(
                this,
                NSLayoutAttribute.CenterY,
                NSLayoutRelation.Equal,
                Superview,
                NSLayoutAttribute.CenterY,
                1,
                0).Active = true;

            _heightConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.Equal,
                    1,
                    Superview.Frame.Height);
            _heightConstraint.Active = true;

            var contentView = Superview.Subviews.First();

            if (Placement == SwipePlacement.Left)
            {
                var leadingToSuperviewLeadingConstraint = NSLayoutConstraint.Create(
                        this,
                        NSLayoutAttribute.Leading,
                        NSLayoutRelation.Equal,
                        Superview,
                        NSLayoutAttribute.Leading,
                        1,
                        0);

                leadingToSuperviewLeadingConstraint.Priority = ConstraintsConstants.VeryHighPriority;
                leadingToSuperviewLeadingConstraint.Active = true;

                var leadingConstraint =
                    Superview.Constraints.FirstOrDefault(x => x.FirstAttribute == NSLayoutAttribute.Leading);

                var trailingToContentViewLeadingConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Trailing,
                    NSLayoutRelation.Equal,
                    contentView,
                    NSLayoutAttribute.Leading,
                    1,
                    leadingConstraint != null
                        ? -leadingConstraint.Constant
                        : 0);

                trailingToContentViewLeadingConstraint.Priority = ConstraintsConstants.VeryHighPriority;
                trailingToContentViewLeadingConstraint.Active = true;
            }
            else
            {
                var trailingToSuperviewTrailingConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Trailing,
                    NSLayoutRelation.Equal,
                    Superview,
                    NSLayoutAttribute.Trailing,
                    1,
                    0);

                trailingToSuperviewTrailingConstraint.Priority = ConstraintsConstants.VeryHighPriority;
                trailingToSuperviewTrailingConstraint.Active = true;

                var trailingConstraint =
                    Superview.Constraints.FirstOrDefault(x => x.FirstAttribute == NSLayoutAttribute.Trailing);

                var leadingToContentViewTrailingConstraints = NSLayoutConstraint.Create(
                        this,
                        NSLayoutAttribute.Leading,
                        NSLayoutRelation.Equal,
                        contentView,
                        NSLayoutAttribute.Trailing,
                        1,
                        trailingConstraint?.Constant ?? 0);

                leadingToContentViewTrailingConstraints.Priority = ConstraintsConstants.VeryHighPriority;
                leadingToContentViewTrailingConstraints.Active = true;
            }
        }

        public void AddItem(SwipeMenuBase subview)
        {
            subview.TranslatesAutoresizingMaskIntoConstraints = false;
            var prevView = Subviews.LastOrDefault();
            _items.Add(subview);
            Add(subview);

            CreateVerticalConstraints(subview);

            if (subview.TreatAsSingleAction)
            {
                CreateSideConstraint(subview);
                CreateWidthConstraintsIfNeeded(subview);
                CreateExpandedViewConstraintIfNeeded(subview);
                return;
            }

            if (prevView == null)
            {
                CreateSideConstraint(subview);
                CreateWidthConstraintsIfNeeded(subview);
            }
            else
            {
                //next elements have ItemWidth as width
                AddConstraint(NSLayoutConstraint.Create(
                                  subview,
                                  NSLayoutAttribute.Width,
                                  NSLayoutRelation.Equal,
                                  1,
                                  ItemWidth));

                if (_expandedViewConstraint != null)
                    _expandedViewConstraint.Active = false;

                CreateExpandedViewConstraintIfNeeded(subview);

                var leftSideConstraint = Placement == SwipePlacement.Left
                    ? NSLayoutAttribute.Trailing
                    : NSLayoutAttribute.Leading;
                var rightSideConstraint = Placement == SwipePlacement.Left
                    ? NSLayoutAttribute.Leading
                    : NSLayoutAttribute.Trailing;

                AddConstraint(
                    NSLayoutConstraint.Create(
                        subview,
                        rightSideConstraint,
                        NSLayoutRelation.Equal,
                        prevView,
                        leftSideConstraint,
                        1,
                        0));
            }
        }

        private void CreateVerticalConstraints(SwipeMenuBase subview)
        {
            AddConstraint(
                NSLayoutConstraint.Create(
                    subview,
                    NSLayoutAttribute.Top,
                    NSLayoutRelation.Equal,
                    this,
                    NSLayoutAttribute.Top,
                    1,
                    0));

            AddConstraint(
                NSLayoutConstraint.Create(
                    subview,
                    NSLayoutAttribute.Bottom,
                    NSLayoutRelation.Equal,
                    this,
                    NSLayoutAttribute.Bottom,
                    1,
                    0));
        }

        private void CreateSideConstraint(UIView subview)
        {
            var constraintSide = Placement == SwipePlacement.Left
                ? NSLayoutAttribute.Leading
                : NSLayoutAttribute.Trailing;

            AddConstraint(
                NSLayoutConstraint.Create(
                    subview,
                    constraintSide,
                    NSLayoutRelation.Equal,
                    this,
                    constraintSide,
                    1,
                    0));
        }

        private void CreateWidthConstraintsIfNeeded(UIView subview)
        {
            if (_smallViewConstraint == null)
            {
                _smallViewConstraint = NSLayoutConstraint.Create(
                    subview,
                    NSLayoutAttribute.Width,
                    NSLayoutRelation.Equal,
                    1,
                    ItemWidth);
                _smallViewConstraint.Active = true;
            }

            if (_expandedWidthViewConstraint == null)
            {
                _expandedWidthViewConstraint = NSLayoutConstraint.Create(
                    subview,
                    NSLayoutAttribute.Width,
                    NSLayoutRelation.GreaterThanOrEqual,
                    1,
                    ItemWidth);
            }
        }

        private void CreateExpandedViewConstraintIfNeeded(SwipeMenuBase subview)
        {
            var leftSideConstraint = Placement == SwipePlacement.Left
                ? NSLayoutAttribute.Trailing
                : NSLayoutAttribute.Leading;

            _expandedViewConstraint = NSLayoutConstraint.Create(
                subview,
                leftSideConstraint,
                NSLayoutRelation.Equal,
                this,
                leftSideConstraint,
                1,
                0);
        }

        public float GetPointToSnap(nfloat offsetX)
        {
            var width = (float)Superview.Frame.Width;

            offsetX = offsetX > 0 ? offsetX : -offsetX;

            if (offsetX < ItemWidth)
                return 0;

            float animationPoint = 0;

            if (offsetX < width * EdgePercent || !FullSwipeAvailable)
                animationPoint = Subviews.Length * ItemWidth;
            else
                animationPoint = width + (Subviews.Length - 1) * ItemWidth + 1;

            return Placement == SwipePlacement.Left
                ? animationPoint
                : -animationPoint;
        }
    }
}