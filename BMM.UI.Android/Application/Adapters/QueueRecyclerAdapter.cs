using _Microsoft.Android.Resource.Designer;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.Adapters.Swipes;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters;

public class QueueRecyclerAdapter : BaseSwipeMenuAdapter
{
    public QueueRecyclerAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
    {
    }
    
    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);
        var view = itemBindingContext.BindingInflate(viewType, parent, false);
        view.SetBackgroundColor(parent.Context.GetColorFromResource(ResourceConstant.Color.background_one_color));
        return new QueueItemViewHolder(view, itemBindingContext, this);
    }
}