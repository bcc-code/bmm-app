using AndroidX.RecyclerView.Widget;

namespace BMM.UI.Droid.Application.Adapters.DragAndDrop
{
    public interface IOnStartDragListener
    {
        void OnStartDrag(RecyclerView.ViewHolder viewHolder);
    }
}