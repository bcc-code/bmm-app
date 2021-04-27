namespace BMM.UI.Droid.Application.Helpers.Gesture
{
    public class SwipeEvent
    {
        public SwipeEvent(SwipeEventType eventType, SwipeDirection direction)
        {
            EventType = eventType;
            Direction = direction;
        }

        public SwipeEventType EventType { get; set; }
        public SwipeDirection Direction { get; set; }
    }
}