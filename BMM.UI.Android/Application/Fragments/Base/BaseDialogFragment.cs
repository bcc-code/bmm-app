using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Storage;
using BMM.Core.ViewModels.Base;
using BMM.UI.Droid.Application.Activities;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views.Fragments;
using ThemeUtils = BMM.UI.Droid.Utils.ThemeUtils;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace BMM.UI.Droid.Application.Fragments.Base
{
    public abstract class BaseDialogFragment<TViewModel>
        : MvxDialogFragment<TViewModel>, INotifyPropertyChanged
        where TViewModel : BaseViewModel
    {
        private Toolbar _toolbar;

        protected BaseDialogFragment()
        {
            RetainInstance = true;
        }

        protected View FragmentView { get; private set; }

        protected virtual int? WindowAnimationsStyle => Resource.Style.FadeInAlertDialogAnimation;
        public virtual bool ShouldShowKeyboardOnStart => false;
        protected Toolbar Toolbar => _toolbar ??= FragmentView.FindViewById<Toolbar>(Resource.Id.toolbar);

        protected abstract int FragmentId { get; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            int? style = ThemeUtils.GetStyleForTheme(AppSettings.SelectedColorTheme, true);

            if (style.HasValue)
                SetStyle(StyleNormal, style.Value);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            Dialog.Window!.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            
            if (WindowAnimationsStyle.HasValue)
                Dialog.Window!.Attributes!.WindowAnimations = WindowAnimationsStyle.Value;
            
            base.OnCreateView(inflater, container, savedInstanceState);
            FragmentView = this.BindingInflate(FragmentId, null);
            SetSize();
            InitToolbar(Toolbar);

            if (ShouldShowKeyboardOnStart)
                Dialog.Window.SetSoftInputMode(SoftInput.AdjustResize | SoftInput.StateAlwaysVisible);
            else
                Dialog.Window.SetSoftInputMode(SoftInput.AdjustResize);

            if (DialogBackgroundColor == default)
                DialogBackgroundColor = Activity.GetColorFromResource(Resource.Color.background_one_color);
                
            Dialog.Window!.DecorView.SystemUiVisibility = DialogBackgroundColor.GetStatusBArVisibilityBasedOnColor(Dialog.Window);  
            
            Bind();
            return FragmentView;
        }

        public MainActivity ParentActivity => (MainActivity)Activity;
        
        protected virtual Color DialogBackgroundColor { get; private set; }
        
        protected virtual void InitToolbar(Toolbar toolbar)
        {
            string title = ViewModel.TextSource[ViewModelUtils.GetVMTitleKey(ViewModel.GetType())];
            
            if (toolbar == null)
                return;
            
            toolbar.Title = title;
            ParentActivity.SetSupportActionBar(toolbar);
            ParentActivity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }
        
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            menu.Clear();
            base.OnCreateOptionsMenu(menu, inflater);
        }
        
        public override void OnStop()
        {
            base.OnStop();
            DetachEvents();
        }

        public override void OnStart()
        {
            base.OnStart();
            AttachEvents();
            SetSize();
        }

        protected virtual void AttachEvents() { }
        
        protected virtual void DetachEvents() { }

        private void SetSize()
        {
            Dialog.Window?.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Bind() => Expression.Empty();
    }
}