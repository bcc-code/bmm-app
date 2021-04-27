using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;
using BMM.UI.Droid.Application.Adapters.DragAndDrop;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    public class DocumentDragAndDropAdapter : MvxRecyclerAdapter, IDragAndDropAdapter
    {
        public DocumentDragAndDropAdapter(IMvxAndroidBindingContext bindingContext, IOnStartDragListener dragStartListener) : base(bindingContext)
        {
            _mDragStartListener = dragStartListener;
        }

        private DocumentsViewModel DataContext => (DocumentsViewModel)BindingContext.DataContext;

        private readonly IOnStartDragListener _mDragStartListener;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);

            return new DragAndDropItemViewHolder(InflateViewForHolder(parent, viewType, itemBindingContext), itemBindingContext);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            base.OnBindViewHolder(holder, position);
            var itemHolder = (DragAndDropItemViewHolder)holder;
            itemHolder.DeleteView.SetOnClickListener(new ListDeleteClickListener<Document>(this, DataContext.Documents, position));
            itemHolder.HandleView.SetOnTouchListener(new CustomTouchListener(itemHolder, _mDragStartListener));
        }

        public bool OnItemMove(int fromPosition, int toPosition)
        {
            DataContext.Documents.Move(fromPosition, toPosition);

            return true;
        }
    }
}