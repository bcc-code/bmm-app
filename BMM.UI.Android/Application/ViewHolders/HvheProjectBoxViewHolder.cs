using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.ItemDecorators;
using BMM.UI.Droid.Application.Listeners;
using FFImageLoading.Cross;
using FFImageLoading.Extensions;
using Microsoft.Maui.Devices;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using LinearLayout = Android.Widget.LinearLayout;

namespace BMM.UI.Droid.Application.ViewHolders;

public class HvheProjectBoxViewHolder : MvxRecyclerViewHolder
{
    private const int ItemSpacing = 12;
    private const int SideSpacing = 116;
    private MvxRecyclerView _achievementsRecyclerView;
    private LinearLayoutManager _layoutManager;
    private IParcelable _state;

    public HvheProjectBoxViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
    {
        this.DelayBind(Bind);
    }

    private void Bind()
    {
        _achievementsRecyclerView = ItemView.FindViewById<MvxRecyclerView>(Resource.Id.AchievementsRecyclerView);
        _achievementsRecyclerView!.NestedScrollingEnabled = false;

        _layoutManager = new LinearLayoutManager(ItemView.Context, LinearLayoutManager.Horizontal, false)
        {
            RecycleChildrenOnDetach = true
        };

        var spacingItemDecoration = new SpacingItemDecoration(ItemSpacing.DpToPixels(), sideSpacing: SideSpacing.DpToPixels());
        var adapter = new TilesCollectionRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
            
        _achievementsRecyclerView!.AddItemDecoration(spacingItemDecoration);
        _achievementsRecyclerView.HasFixedSize = true;
        _achievementsRecyclerView.SetLayoutManager(_layoutManager);
        _achievementsRecyclerView.Adapter = adapter;
    }
    
    public override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        _layoutManager.OnRestoreInstanceState(_state);
    }

    public override void OnDetachedFromWindow()
    {
        _state = _layoutManager.OnSaveInstanceState();
        base.OnDetachedFromWindow();
    }
}