using Android.Views;

namespace BMM.UI.Droid.Application.Adapters.DragAndDrop
{
    public class CustomTouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        private readonly DragAndDropItemViewHolder _dragAndDropItemViewHolder;
        private readonly IOnStartDragListener _mDragStartListener;

        public CustomTouchListener(DragAndDropItemViewHolder dragAndDropItemViewHolder, IOnStartDragListener mDragStartListener)
        {
            _dragAndDropItemViewHolder = dragAndDropItemViewHolder;
            _mDragStartListener = mDragStartListener;
        }

        public bool OnTouch(View view, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                _mDragStartListener.OnStartDrag(_dragAndDropItemViewHolder);
            }

            return false;
        }
    }
}