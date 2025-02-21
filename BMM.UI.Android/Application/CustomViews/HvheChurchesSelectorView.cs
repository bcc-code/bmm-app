using _Microsoft.Android.Resource.Designer;
using Android.Content;
using Android.Runtime;
using Android.Util;
using BMM.Core.Models.POs.BibleStudy;
using BMM.UI.Droid.Application.Bindings;
using BMM.UI.Droid.Application.Extensions;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.Droid.Application.CustomViews
{
    [Register("bmm.ui.droid.application.customViews.HvheChurchesSelectorView")]
    public class HvheChurchesSelectorView
        : FrameLayout,
          IMvxBindingContextOwner
    {
        private HvheChurchesSelectorPO _hvheChurchesSelector;
        private TextView _leftItemLabel;
        private TextView _rightItemLabel;
        private bool _isLeftItemSelected;
        private bool _isRightItemSelected;

        protected HvheChurchesSelectorView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public HvheChurchesSelectorView(Context context)
            : base(context)
        {
            Initialize(context);
        }

        public HvheChurchesSelectorView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize(context);
        }

        public HvheChurchesSelectorView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Initialize(context);
        }
        
        public HvheChurchesSelectorPO HvheChurchesSelector
        {
            get => _hvheChurchesSelector;
            set
            {
                _hvheChurchesSelector = value;
                if (BindingContext != null)
                    BindingContext.DataContext = value;
            }
        }

        public IMvxBindingContext BindingContext { get; set; }

        private void CreateBinding()
        {
            _leftItemLabel = FindViewById<TextView>(ResourceConstant.Id.LeftItemLabel);
            _rightItemLabel = FindViewById<TextView>(ResourceConstant.Id.RightItemLabel);
            
            var set = this.CreateBindingSet<HvheChurchesSelectorView, HvheChurchesSelectorPO>();
            
            set.Bind(this)
                .For(v => v.IsLeftItemSelected)
                .To(po => po.IsLeftItemSelected);
            
            set.Bind(this)
                .For(v => v.IsRightItemSelected)
                .To(po => po.IsRightItemSelected);
            
            set.Apply();
        }

        public bool IsRightItemSelected
        {
            get => _isRightItemSelected;
            set
            {
                _isRightItemSelected = value;
                _rightItemLabel.SetTextColor(_isRightItemSelected
                    ? Context.GetColorFromResource(ResourceConstant.Color.label_one_color)
                    : Context.GetColorFromResource(ResourceConstant.Color.label_three_color));
            }
        }

        public bool IsLeftItemSelected
        {
            get => _isLeftItemSelected;
            set
            {
                _isLeftItemSelected = value;
                _leftItemLabel.SetTextColor(_isLeftItemSelected
                    ? Context.GetColorFromResource(ResourceConstant.Color.label_one_color)
                    : Context.GetColorFromResource(ResourceConstant.Color.label_three_color));
            }
        }
        
        private void Initialize(Context context)
        {
            this.LayoutAndAttachBindingContextOrDesignMode(
                context,
                Resource.Layout.view_churches_selector,
                () => this.DelayBind(CreateBinding));
        }
    }
}