using Android.Content;
using Android.Views;
using BMM.Core.Models;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;

namespace BMM.UI.Droid.Application.Adapters
{
    public class TrackInfoAdapter: MvxAdapter<IListItem>
    {
        public TrackInfoAdapter(Context context, IMvxAndroidBindingContext bindingContext)
            : base(context, bindingContext)
        {
        }

        protected override View GetBindableView(View convertView, object source, ViewGroup parentView, int templateId)
        {
            if (source is SectionHeader)
            {
                templateId = Resource.Layout.listitem_header;
            } else if (source is ExternalRelationListItem)
            {
                templateId = Resource.Layout.listitem_trackrelation_external;
            } else if (source is SelectableListItem)
            {
                templateId = Resource.Layout.listitem;
            }

            return base.GetBindableView(convertView, source, parentView, templateId);
        }
    }
}
