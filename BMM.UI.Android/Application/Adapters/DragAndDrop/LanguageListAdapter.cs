using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters.DragAndDrop
{
    public class LanguageListAdapter: MvxRecyclerAdapter, IDragAndDropAdapter
    {
        private readonly IOnStartDragListener _mDragStartListener;

        private LanguageContentViewModel DataContext => (LanguageContentViewModel)BindingContext.DataContext;


        public LanguageListAdapter(IMvxAndroidBindingContext bindingContext, IOnStartDragListener dragStartListener) : base(bindingContext)
        {
            _mDragStartListener = dragStartListener;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);

            return new DragAndDropItemViewHolder(InflateViewForHolder(parent, viewType, itemBindingContext), itemBindingContext);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            base.OnBindViewHolder(holder, position);
            var itemHolder = (DragAndDropItemViewHolder)holder;
            itemHolder.HandleView.SetOnTouchListener(new CustomTouchListener(itemHolder, _mDragStartListener));
        }

        public void OnItemAdd(int position)
        {
            NotifyItemInserted(DataContext.Languages.Count - 1);
        }

        public void OnItemDismiss(int position)
        {
            DataContext.Languages.RemoveAt(position);

            NotifyItemRemoved(position);
        }

        public bool OnItemMove(int fromPosition, int toPosition)
        {
            DataContext.Languages.Move(fromPosition, toPosition);

            NotifyItemMoved(fromPosition, toPosition);
            return true;
        }
    }
}