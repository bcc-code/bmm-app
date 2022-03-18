using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    public class ContinueAudiobookCarouselRecyclerAdapter : MvxRecyclerAdapter
    {
        public ContinueAudiobookCarouselRecyclerAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }
        
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext!.LayoutInflaterHolder);
            var view = itemBindingContext.BindingInflate(viewType, parent, false);
            return new ContinueListeningTileViewHolder(view, itemBindingContext);
        }
    }
}