using System;
using Android.Views;

namespace BMM.UI.Droid.Application.Helpers.Gesture
{
    public class HorizontalSwipeDetector: Java.Lang.Object, View.IOnTouchListener
    {
        private const float MinThreshold = 120;

        private float _initialX;

        private SwipeDirection _currentSwipeDirection;

        public bool OnTouch(View view, MotionEvent motionEvent)
        {
            switch (motionEvent.Action)
            {
                case MotionEventActions.Down:
                    _initialX = motionEvent.GetX();

                    _currentSwipeDirection = SwipeDirection.None;
                    break;
                case MotionEventActions.Move:
                    var deltaX = motionEvent.GetX() - _initialX;
                    if (Math.Abs(deltaX) > MinThreshold)
                    {
                        _currentSwipeDirection = motionEvent.GetX() < _initialX
                            ? SwipeDirection.Right : SwipeDirection.Left;

                        DispatchSwipeEvent(SwipeEventType.Move);
                    }
                    break;
                case MotionEventActions.Up:

                    if (_currentSwipeDirection != SwipeDirection.None)
                    {
                        DispatchSwipeEvent(SwipeEventType.End);
                    }

                    break;
                case MotionEventActions.Cancel:
                    DispatchSwipeEvent(SwipeEventType.End);
                    break;
            }

            return true;
        }

        private void DispatchSwipeEvent(SwipeEventType eventType)
        {
            var swipeEvent = new SwipeEvent(eventType, _currentSwipeDirection);
            OnSwipeDetected?.Invoke(this, swipeEvent);
        }

        public event EventHandler<SwipeEvent> OnSwipeDetected;
    }
}