using System;
using System.ComponentModel;
using Airbnb.Lottie;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    [Register(nameof(LottieButton)), DesignTimeVisible(true)]
    public class LottieButton : UIButton
    {
        private LOTAnimationView _animationView;

        public LottieButton()
        {
        }

        public LottieButton(IntPtr handle) : base(handle)
        {
        }
        
        public void AddAnimation(LOTAnimationView animationView)
        {
            _animationView?.RemoveFromSuperview();
            _animationView = animationView;

            AddSubview(_animationView);

            _animationView.TranslatesAutoresizingMaskIntoConstraints = false;

            NSLayoutConstraint.ActivateConstraints(
            new[]
            {
                _animationView.LeadingAnchor.ConstraintEqualTo(ImageView.LeadingAnchor),
                _animationView.TrailingAnchor.ConstraintEqualTo(ImageView.TrailingAnchor),
                _animationView.TopAnchor.ConstraintEqualTo(ImageView.TopAnchor),
                _animationView.BottomAnchor.ConstraintEqualTo(ImageView.BottomAnchor)
            });

            _animationView.Hidden = true;
        }

        public void PlayAnimation()
        {
            if (_animationView == null)
                return;

            _animationView.Hidden = false;
            _animationView.LoopAnimation = true;
            _animationView.Play();
            _animationView.BringSubviewToFront(this);
        }
        
        public void StopAnimation()
        {
            if (_animationView == null)
                return;

            _animationView.Hidden = true;
            _animationView.Stop();
        }
    }
}