using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters;

public class SearchResultsRecyclerAdapter : LoadMoreRecyclerAdapter
{
    public SearchResultsRecyclerAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
    {
    }
    
    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext!.LayoutInflaterHolder);
        var view = itemBindingContext.BindingInflate(viewType, parent, false);

        return viewType switch
        {
            Resource.Layout.listitem_highlighted_text_track => new HighlightedTextTrackViewHolder(view, itemBindingContext),
            _ => new MvxRecyclerViewHolder(view, itemBindingContext)
        };
    }
}