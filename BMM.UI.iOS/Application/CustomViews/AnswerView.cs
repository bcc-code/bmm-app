using System;
using System.ComponentModel;
using System.Drawing;
using BMM.Api.Implementation.Models;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS.CustomViews
{
    [DesignTimeVisible(true)]
    public partial class AnswerView : MvxView
    {
        private readonly Action<AnswerView> _action;
        public static readonly UINib Nib = UINib.FromName(nameof(AnswerView), NSBundle.MainBundle);
        public static readonly NSString Key = new NSString(nameof(AnswerView));

        public AnswerView(Action<AnswerView> action)
        {
            _action = action;
            Initialize();
        }
        
        private void Initialize()
        {
            this.LoadXib(true);
            this.CreateBindingContext();
            this.DelayBind(Bind);
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<AnswerView, Answer>();

            set.Bind(AnswerLabel)
                .To(po => po.Text);
            
            set.Bind(AnswerLetterLabel)
                .To(po => po.Id);
            
            set.Apply();
            SetThemes();
        }

        private void SetThemes()
        {
            AnswerLabel.ApplyTextTheme(AppTheme.Subtitle2Label1);
            ContainerView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                _action.Invoke(this);
                ShakeAnimation(ContainerView);
            }));
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ContainerView.Layer.CornerRadius = 16;
        }
        
        private void BounceAnimation(UIView view)
        {
            var animation = CAKeyFrameAnimation.FromKeyPath("position");

            var originalPosition = view.Layer.Position;
            var bounceHeight = 10f;
            var bounceDuration = 0.3;

            var bouncePoints = new NSValue[]
            {
                NSValue.FromCGPoint(originalPosition),
                NSValue.FromCGPoint(new CGPoint(originalPosition.X, originalPosition.Y - bounceHeight)),
                NSValue.FromCGPoint(originalPosition),
            };

            animation.Values = bouncePoints;
            animation.Duration = bounceDuration;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
    
            animation.RemovedOnCompletion = true;
            view.Layer.AddAnimation(animation, "bounceAnimation");

            view.Layer.Position = originalPosition;
        }
        
        private void ShakeAnimation(UIView view)
        {
            var animation = CAKeyFrameAnimation.FromKeyPath("position");
            
            var originalPosition = view.Layer.Position;
            var shakeDistance = 10f;
            var shakeDuration = 0.3;
            
            var shakePoints = new NSValue[]
            {
                NSValue.FromCGPoint(originalPosition),
                NSValue.FromCGPoint(new CGPoint(originalPosition.X - shakeDistance, originalPosition.Y)),
                NSValue.FromCGPoint(originalPosition),
                NSValue.FromCGPoint(new CGPoint(originalPosition.X + shakeDistance, originalPosition.Y)),
                NSValue.FromCGPoint(originalPosition)
            };
        
            animation.Values = shakePoints;
            animation.Duration = shakeDuration;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
            animation.RemovedOnCompletion = true;
        
            view.Layer.AddAnimation(animation, "shakeAnimation");
            view.Layer.Position = originalPosition;
        }

        public void SetAlphaToBackground()
        {
            ContainerView.BackgroundColor = AppColors.BackgroundOneColor.ColorWithAlpha(0.2f);
        }
    }
}