using BMM.UI.Droid.Application.Helpers.Gesture;
using Google.Android.Material.BottomSheet;

namespace BMM.UI.Droid.Application.Helpers.BottomSheet
{
    /// <summary>
    /// This Blocker prevents the BottomSheet from being dragged while a swipe is happening.
    /// One use case is to prevent the BottomSheet from being dragged while swiping to the next song.
    /// </summary>
    public class PreventBottomSheetChangesWhileSwipeHappens
    {
        private readonly BottomSheetManager _bottomSheetManager;
        private readonly HorizontalSwipeDetector _swipeDetector;

        private int _previousState;
        private bool _blockBottomSheetDragging;

        public PreventBottomSheetChangesWhileSwipeHappens(BottomSheetManager bottomSheetManager,
            HorizontalSwipeDetector swipeDetector)
        {
            _bottomSheetManager = bottomSheetManager;
            _swipeDetector = swipeDetector;
        }

        public void Register()
        { 
            _swipeDetector.OnSwipeDetected += (sender, swipeEvent) =>
            {
                if (swipeEvent.EventType == SwipeEventType.Move)
                {
                    _blockBottomSheetDragging = true;
                }

                if (swipeEvent.EventType == SwipeEventType.End)
                {
                    _blockBottomSheetDragging = false;
                }
            };

            _bottomSheetManager.OnBottomSheetStateChanged += (sender, state) =>
            {
                if (state == BottomSheetBehavior.StateDragging)
                {
                    if (_blockBottomSheetDragging)
                    {
                        _bottomSheetManager.ResetStateChange(_previousState);
                    }
                }
                else
                {
                    var bottomSheetIsExpanded = state == BottomSheetBehavior.StateExpanded;
                    var bottomSheetIsHidden = state == BottomSheetBehavior.StateHidden;

                    if (bottomSheetIsExpanded || bottomSheetIsHidden)
                    {
                        _previousState = state;
                    }
                }
            };
        }
    }
}