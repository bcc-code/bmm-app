using BMM.UI.iOS.Enums;

namespace BMM.UI.iOS.GestureRecognizers
{
    public class PanDirectionGestureRecognizer : UIPanGestureRecognizer
    {
        private readonly PanDirection _direction;

        public PanDirectionGestureRecognizer(PanDirection direction)
        {
            _direction = direction;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            
            if (State == UIGestureRecognizerState.Began)
            {
                var velocity = VelocityInView(View);
                switch (_direction) {
                    case PanDirection.Horizontal when Math.Abs(velocity.Y) > Math.Abs(velocity.X):
                        State = UIGestureRecognizerState.Cancelled;
                        break;
                    case PanDirection.Vertical when Math.Abs(velocity.X) > Math.Abs(velocity.Y):
                        State = UIGestureRecognizerState.Cancelled;
                        break;
                }
            }
        }
    }
}