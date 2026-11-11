using CoreAnimation;
using ObjCRuntime;

namespace BMM.UI.iOS
{
    /// <summary>
    /// A single-line label that scrolls its text in a continuous loop when the text is wider than the label.
    /// Managed replacement for the native MarqueeLabel binding, which had no arm64 simulator slice.
    /// </summary>
    [Register("MarqueeLabel")]
    public class MarqueeLabel : UILabel
    {
        private const string ScrollAnimationKey = "MarqueeLabel.Scroll";
        private const double PauseBeforeScroll = 1.0;
        private const float GapBetweenCopies = 40;

        private UIView _container;
        private UILabel _firstCopy;
        private UILabel _secondCopy;
        private NSObject _foregroundObserver;
        private string _layoutKey;
        private nfloat _scrollDistance;
        private nfloat _fadeLength;
        private double _scrollDuration = 6;

        public MarqueeLabel()
        {
        }

        public MarqueeLabel(CGRect frame) : base(frame)
        {
        }

        public MarqueeLabel(NSCoder coder) : base(coder)
        {
        }

        protected internal MarqueeLabel(NativeHandle handle) : base(handle)
        {
        }

        /// <summary>Width of the fade at both edges while the text is scrolling.</summary>
        public nfloat FadeLength
        {
            get => _fadeLength;
            set
            {
                _fadeLength = value;
                SetNeedsLayout();
            }
        }

        /// <summary>Seconds one full scroll takes.</summary>
        public double ScrollDuration
        {
            get => _scrollDuration;
            set
            {
                _scrollDuration = value;
                SetNeedsLayout();
            }
        }

        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                SetNeedsLayout();
            }
        }

        public override UIFont Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                SetNeedsLayout();
            }
        }

        public override UIColor TextColor
        {
            get => base.TextColor;
            set
            {
                base.TextColor = value;
                SetNeedsLayout();
            }
        }

        public override UITextAlignment TextAlignment
        {
            get => base.TextAlignment;
            set
            {
                base.TextAlignment = value;
                _layoutKey = null;
                SetNeedsLayout();
            }
        }

        // The width comes from constraints; reporting the text width would let long text stretch the label.
        public override CGSize IntrinsicContentSize
            => new CGSize(NoIntrinsicMetric, base.IntrinsicContentSize.Height);

        public override void DrawText(CGRect rect)
        {
            // The copies in _container render the text. Drawing here as well would show it twice.
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            EnsureSubviews();

            _firstCopy.Font = _secondCopy.Font = Font;
            _firstCopy.TextColor = _secondCopy.TextColor = TextColor;

            bool reduceMotion = UIAccessibility.IsReduceMotionEnabled;
            string layoutKey = $"{Text}|{Font?.Name}|{Font?.PointSize}|{Bounds.Width}|{Bounds.Height}|{_fadeLength}|{_scrollDuration}|{reduceMotion}";
            if (layoutKey == _layoutKey)
                return;

            _layoutKey = layoutKey;
            Rebuild(reduceMotion);
        }

        public override void MovedToWindow()
        {
            base.MovedToWindow();

            if (Window == null)
            {
                StopObservingForeground();
                return;
            }

            // Core Animation drops running animations when a view leaves its window or the app is backgrounded.
            _foregroundObserver ??= NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.WillEnterForegroundNotification,
                _ => StartScrolling());
            StartScrolling();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                StopObservingForeground();

            base.Dispose(disposing);
        }

        private void EnsureSubviews()
        {
            if (_container != null)
                return;

            ClipsToBounds = true;

            _container = new UIView { UserInteractionEnabled = false };
            _firstCopy = CreateCopy();
            _secondCopy = CreateCopy();
            _container.AddSubviews(_firstCopy, _secondCopy);
            AddSubview(_container);
        }

        private static UILabel CreateCopy() => new UILabel
        {
            Lines = 1,
            BackgroundColor = UIColor.Clear,
            IsAccessibilityElement = false
        };

        private void Rebuild(bool reduceMotion)
        {
            _container.Layer.RemoveAnimation(ScrollAnimationKey);
            _container.Frame = Bounds;
            _firstCopy.Text = Text;

            var textWidth = NMath.Ceiling(_firstCopy.SizeThatFits(new CGSize(nfloat.MaxValue, Bounds.Height)).Width);
            bool shouldScroll = !reduceMotion && Bounds.Width > 0 && textWidth > Bounds.Width;

            if (!shouldScroll)
            {
                _scrollDistance = 0;
                _firstCopy.Frame = Bounds;
                _firstCopy.TextAlignment = TextAlignment;
                _firstCopy.LineBreakMode = UILineBreakMode.TailTruncation;
                _secondCopy.Hidden = true;
                Layer.Mask = null;
                return;
            }

            // Both copies are inset by the fade length so the start of the text is fully visible while paused.
            _scrollDistance = textWidth + GapBetweenCopies;
            _firstCopy.TextAlignment = UITextAlignment.Natural;
            _firstCopy.Frame = new CGRect(_fadeLength, 0, textWidth, Bounds.Height);
            _secondCopy.Text = Text;
            _secondCopy.TextAlignment = UITextAlignment.Natural;
            _secondCopy.Frame = new CGRect(_fadeLength + _scrollDistance, 0, textWidth, Bounds.Height);
            _secondCopy.Hidden = false;

            Layer.Mask = CreateFadeMask();
            StartScrolling();
        }

        private CAGradientLayer CreateFadeMask()
        {
            if (_fadeLength <= 0 || Bounds.Width <= _fadeLength * 2)
                return null;

            double fadeFraction = _fadeLength / Bounds.Width;
            var transparent = UIColor.Clear.CGColor;
            var opaque = UIColor.Black.CGColor;

            return new CAGradientLayer
            {
                Frame = Bounds,
                StartPoint = new CGPoint(0, 0.5),
                EndPoint = new CGPoint(1, 0.5),
                Colors = new[] { transparent, opaque, opaque, transparent },
                Locations = new[]
                {
                    NSNumber.FromDouble(0),
                    NSNumber.FromDouble(fadeFraction),
                    NSNumber.FromDouble(1 - fadeFraction),
                    NSNumber.FromDouble(1)
                }
            };
        }

        private void StartScrolling()
        {
            if (_container == null || _scrollDistance <= 0 || Window == null || _scrollDuration <= 0)
                return;

            var scroll = CABasicAnimation.FromKeyPath("transform.translation.x");
            scroll.From = NSNumber.FromDouble(0);
            scroll.To = NSNumber.FromDouble(-_scrollDistance);
            scroll.BeginTime = PauseBeforeScroll;
            scroll.Duration = _scrollDuration;
            scroll.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);

            // After one scroll the second copy sits exactly where the first one started, so the loop is seamless.
            var loop = new CAAnimationGroup
            {
                Animations = new CAAnimation[] { scroll },
                Duration = PauseBeforeScroll + _scrollDuration,
                RepeatCount = float.PositiveInfinity,
                RemovedOnCompletion = false
            };

            _container.Layer.RemoveAnimation(ScrollAnimationKey);
            _container.Layer.AddAnimation(loop, ScrollAnimationKey);
        }

        private void StopObservingForeground()
        {
            if (_foregroundObserver == null)
                return;

            NSNotificationCenter.DefaultCenter.RemoveObserver(_foregroundObserver);
            _foregroundObserver.Dispose();
            _foregroundObserver = null;
        }
    }
}
