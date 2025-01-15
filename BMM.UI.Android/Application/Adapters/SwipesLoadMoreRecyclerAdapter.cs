using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels.Base;
using BMM.UI.Droid.Application.Adapters.Swipes;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters;

public class SwipesLoadMoreRecyclerAdapter : BaseSwipeMenuAdapter
{
    protected ILoadMoreDocumentsViewModel ViewModel => BindingContext.DataContext as ILoadMoreDocumentsViewModel;

    private bool ShouldDisplayBottomSpinner => ViewModel?.IsFullyLoaded == false;
    
    public SwipesLoadMoreRecyclerAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
    {
    }

    public override object GetItem(int position)
    {
        if (ShouldDisplayBottomSpinner && position + 1 == ItemCount)
            return null;
        else
            return base.GetItem(position);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (ShouldDisplayBottomSpinner && position == ItemCount - 1)
        {
            ((IMvxRecyclerViewHolder) holder).DataContext = ViewModel;
        }
        else
        {
            base.OnBindViewHolder(holder, position);
        }
    }
}