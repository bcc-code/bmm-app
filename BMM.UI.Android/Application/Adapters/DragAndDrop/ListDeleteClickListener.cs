using Android.Views;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Adapters.DragAndDrop
{
    public class ListDeleteClickListener<TCollectionItem>: Java.Lang.Object, View.IOnClickListener
    {
        private readonly MvxRecyclerAdapter _adapter;
        private readonly MvxObservableCollection<TCollectionItem> _collection;
        private readonly int _position;

        public ListDeleteClickListener(MvxRecyclerAdapter adapter, MvxObservableCollection<TCollectionItem> collection, int position)
        {
            _adapter = adapter;
            _collection = collection;
            _position = position;
        }
        public void OnClick(View view)
        {
            _collection.RemoveAt(_position);
            _adapter.NotifyItemRemoved(_position);
        }
    }
}