using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.CustomViews.TabLayout.Base;
using BMM.UI.Droid.Application.Decorators;

namespace BMM.UI.Droid.Application.CustomViews.TabLayout
{
    [Register("bmm.ui.droid.application.customViews.FlexibleWidthTabLayout")]
    public class FlexibleWidthTabLayout : TabLayoutBase
    {
        private FlexibleWidthTabTabLayoutAdapter _flexibleWidthTabTabLayoutAdapter;
        private HorizontalSpacingItemDecoration _horizontalSpacingItemDecorator;

        public FlexibleWidthTabLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(
            javaReference,
            transfer)
        {
        }

        public FlexibleWidthTabLayout(Context context) : base(context)
        {
        }

        public FlexibleWidthTabLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        protected override BaseTabLayoutAdapter Adapter
            => _flexibleWidthTabTabLayoutAdapter ??= new FlexibleWidthTabTabLayoutAdapter();

        protected override int LayoutId => Resource.Layout.view_tab_layout;

        protected override int ItemSpacing => Resources!.GetDimensionPixelSize(Resource.Dimension.margin_xxsmall);

        protected override RecyclerView.ItemDecoration ItemDecorator
        {
            get
            {
                if (_horizontalSpacingItemDecorator != null)
                    return _horizontalSpacingItemDecorator;

                int regularSpacing = Resources!.GetDimensionPixelSize(Resource.Dimension.margin_xxsmall);
                int additionalHorizontalMargin = Resources!.GetDimensionPixelSize(Resource.Dimension.margin_xxsmall);

                _horizontalSpacingItemDecorator = new HorizontalSpacingItemDecoration(
                    regularSpacing,
                    additionalHorizontalMargin);

                return _horizontalSpacingItemDecorator;
            }
        }
    }
}