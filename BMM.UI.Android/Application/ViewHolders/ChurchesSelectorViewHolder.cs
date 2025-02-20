using System;
using _Microsoft.Android.Resource.Designer;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BMM.Core.Interactions.Base;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.YearInReview;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders
{
    public class ChurchesSelectorViewHolder : MvxRecyclerViewHolder
    {
        public ChurchesSelectorViewHolder(
            View itemView,
            IMvxAndroidBindingContext context) : base(itemView, context)
        {
        }
    }
}