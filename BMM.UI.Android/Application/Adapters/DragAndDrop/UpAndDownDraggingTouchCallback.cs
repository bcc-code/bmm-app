using System;
using AndroidX.RecyclerView.Widget;

namespace BMM.UI.Droid.Application.Adapters.DragAndDrop
{
    public class UpAndDownDraggingTouchCallback : ItemTouchHelper.Callback
    {
        private readonly IDragAndDropAdapter _adapter;

        public override bool IsItemViewSwipeEnabled => false;

        public override bool IsLongPressDragEnabled => true;

        public UpAndDownDraggingTouchCallback(IDragAndDropAdapter adapter)
        {
            _adapter = adapter;
        }

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            const int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            return MakeMovementFlags(dragFlags, 0);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            return _adapter.OnItemMove(viewHolder.AdapterPosition, target.AdapterPosition);
        }

        public override void OnSwiped(RecyclerView.ViewHolder p0, int p1)
        {
            throw new NotSupportedException();
        }

        public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
        {
            if (actionState != ItemTouchHelper.ActionStateIdle)
            {
                if (viewHolder is DragAndDropItemViewHolder itemViewHolder)
                    itemViewHolder.OnItemSelected();
            }

            base.OnSelectedChanged(viewHolder, actionState);
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            base.ClearView(recyclerView, viewHolder);
            viewHolder.ItemView.Alpha = 1.0f;

            if (viewHolder is DragAndDropItemViewHolder itemViewHolder)
                itemViewHolder.OnItemClear();
        }
    }
}