using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.UI.Droid.Application.ViewHolders;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Adapters
{
    public class BibleStudyRecyclerAdapter : MvxRecyclerAdapter
    {
        public BibleStudyRecyclerAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext!.LayoutInflaterHolder);
            var view = itemBindingContext.BindingInflate(viewType, parent, false);

            switch (viewType)
            {
                case Resource.Layout.listitem_bible_study_progress:
                    return new BibleStudyProgressCollectionViewHolder(view, itemBindingContext);
                case Resource.Layout.listitem_extrenal_relations_play_button:
                    return new BibleStudyExternalRelationsPlayButtonViewHolder(view, itemBindingContext);
                default:
                    return new MvxRecyclerViewHolder(view, itemBindingContext);
            }
        }
    }
}