using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ViewHolders;

namespace BMM.UI.Droid.Application.Listeners;

public class HvheDetailsScrollListener : RecyclerView.OnScrollListener
{
    private readonly Action<bool> _action;
    private int? _churchesSelectorItemPosition;

    public HvheDetailsScrollListener(
        Action<bool> action)
    {
        _action = action;
    }

    public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
    {
        base.OnScrolled(recyclerView, dx, dy);

        var layoutManager = recyclerView.GetLayoutManager() as LinearLayoutManager;
        if (layoutManager == null)
            return;

        int firstVisiblePosition = layoutManager.FindFirstVisibleItemPosition();

        if (_churchesSelectorItemPosition == null)
        {
            if (layoutManager.FindFirstVisibleItemPosition() == RecyclerView.NoPosition)
                return;

            var viewHolder = recyclerView.FindViewHolderForAdapterPosition(firstVisiblePosition);
                
            if (viewHolder is not ChurchesSelectorViewHolder)
                return;

            _churchesSelectorItemPosition = firstVisiblePosition;
        }
            
        if (firstVisiblePosition > _churchesSelectorItemPosition)
            return;
            
        int[] itemLocation = new int[2];
        int[] recyclerViewLocation = new int[2];
            
        var churchesSelectorViewHolder = recyclerView.FindViewHolderForAdapterPosition(_churchesSelectorItemPosition.Value);
        churchesSelectorViewHolder!.ItemView.GetLocationOnScreen(itemLocation);
        recyclerView.GetLocationOnScreen(recyclerViewLocation);

        int itemTop = itemLocation[1]; 
        int recyclerViewTop = recyclerViewLocation[1];
            
        _action.Invoke(itemTop <= recyclerViewTop);
    }
}