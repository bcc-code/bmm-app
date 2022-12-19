using Android.Views;
using Android.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class FlexibleWidthTabTabLayoutViewHolder : MvxRecyclerViewHolder
    {
        private bool _isSelected;
        private TextView _textView;
        
        public FlexibleWidthTabTabLayoutViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            this.DelayBind(Bind);
        }
        
        private void Bind()
        {
            _textView = ItemView.FindViewById<TextView>(Resource.Id.Title);
            var set = this.CreateBindingSet<FlexibleWidthTabTabLayoutViewHolder, SearchResultsViewModel>();

            set.Bind(this)
                .For(v => v.IsSelected)
                .To(vm => vm.Selected);
            
            set.Apply();
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                if (_isSelected)
                    _textView.SetTextColor(ItemView.Context.GetColorFromResource(Resource.Color.label_primary_color));
                else
                    _textView.SetTextColor(ItemView.Context.GetColorFromResource(Resource.Color.label_tertiary_color));
            }
        }
    }
}