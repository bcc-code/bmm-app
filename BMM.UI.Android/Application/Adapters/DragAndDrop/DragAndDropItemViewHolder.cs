using Android.Graphics;
using Android.Views;
using Android.Widget;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters.DragAndDrop
{
    public class DragAndDropItemViewHolder : MvxRecyclerViewHolder
    {
        public readonly ImageView DeleteView;
        public readonly ImageView HandleView;
        private readonly View _itemView;

        public DragAndDropItemViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            _itemView = itemView;
            DeleteView = itemView.FindViewById<ImageView>(Resource.Id.delete);
            HandleView = itemView.FindViewById<ImageView>(Resource.Id.handle);
        }

        public void OnItemSelected()
        {
            _itemView.SetBackgroundResource(Resource.Color.background_secondary_color);
        }

        public void OnItemClear()
        {
            _itemView.SetBackgroundResource(Resource.Color.background_primary_color);
        }
    }
}