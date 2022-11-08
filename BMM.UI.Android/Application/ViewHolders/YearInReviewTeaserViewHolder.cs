using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.POs.YearInReview;
using BMM.UI.Droid.Application.Adapters;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class YearInReviewTeaserViewHolder : MvxRecyclerViewHolder
    {
        private readonly PodcastContextHeaderRecyclerAdapter _podcastContextHeaderRecyclerAdapter;
        private IBmmInteraction _expandOrCollapseInteraction;

        public YearInReviewTeaserViewHolder(
            View itemView,
            IMvxAndroidBindingContext context,
            PodcastContextHeaderRecyclerAdapter podcastContextHeaderRecyclerAdapter) : base(itemView, context)
        {
            _podcastContextHeaderRecyclerAdapter = podcastContextHeaderRecyclerAdapter;
            this.DelayBind(Bind);
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<YearInReviewTeaserViewHolder, YearInReviewTeaserPO>();

            set.Bind(this)
                .For(v => v.ExpandOrCollapseInteraction)
                .To(po => po.ExpandOrCollapseInteraction);

            set.Apply();
        }
        
        public IBmmInteraction ExpandOrCollapseInteraction
        {
            get => _expandOrCollapseInteraction;
            set
            {
                if (_expandOrCollapseInteraction != null)
                    _expandOrCollapseInteraction.Requested -= ExpandOrCollapseInteractionRequested;

                _expandOrCollapseInteraction = value;

                if (_expandOrCollapseInteraction != null)
                    _expandOrCollapseInteraction.Requested += ExpandOrCollapseInteractionRequested;
            }
        }

        private void ExpandOrCollapseInteractionRequested(object sender, EventArgs e)
        {
            _podcastContextHeaderRecyclerAdapter.NotifyItemChanged(BindingAdapterPosition);
        }
    }
}