using _Microsoft.Android.Resource.Designer;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters;

public class HvheDetailsRecyclerAdapter : MvxRecyclerAdapter
{
    public HvheDetailsRecyclerAdapter(IMvxAndroidBindingContext? bindingContext) : base(bindingContext)
    {
    }
    
    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext!.LayoutInflaterHolder);
        var view = itemBindingContext.BindingInflate(viewType, parent, false);

        return viewType switch
        {
            ResourceConstant.Layout.listitem_boys_vs_girls => new BoysVsGirlsViewHolder(view, itemBindingContext),
            ResourceConstant.Layout.listitem_churches_selector => new ChurchesSelectorViewHolder(view, itemBindingContext),
            ResourceConstant.Layout.listitem_highlighted_church => new HighlightedChurchViewHolder(view, itemBindingContext),
            ResourceConstant.Layout.listitem_standard_church => new StandardChurchViewHolder(view, itemBindingContext),
            _ => new MvxRecyclerViewHolder(view, itemBindingContext)
        };
    }
}