using System;
using Android.Views;
using BMM.Core.Extensions;
using Google.Android.Material.BottomSheet;

namespace BMM.UI.Droid.Application.Helpers.BottomSheet
{
    public class BottomSheetManager: BottomSheetBehavior.BottomSheetCallback
    {
        private readonly BottomSheetBehavior _bottomSheet;

        public BottomSheetManager(BottomSheetBehavior bottomSheet)
        {
            _bottomSheet = bottomSheet;
            _bottomSheet.Hideable = true;
        }

        public override void OnSlide(View bottomSheet, float slideOffset)
        {
        }

        public override void OnStateChanged(View bottomSheet, int state)
        {
            OnBottomSheetStateChanged?.Invoke(bottomSheet, state);
        }

        public void ResetStateChange(int originalState)
        {
            _bottomSheet.State = originalState;
        }

        public void Open()
        {
            _bottomSheet.State = BottomSheetBehavior.StateExpanded;
        }

        public void Close()
        {
            _bottomSheet.State = BottomSheetBehavior.StateHidden;
        }

        public bool IsOpen => _bottomSheet.State.IsNoneOf(BottomSheetBehavior.StateHidden, BottomSheetBehavior.StateCollapsed);
        public event EventHandler<int> OnBottomSheetStateChanged;
    }
}