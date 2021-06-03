using System;
using BMM.UI.iOS.Constants;
using Foundation;
using UIKit;

namespace BMM.UI.iOS
{
    sealed class TextViewWithDescription : UIView
    {
        public UITextField TitleTextField { get; private set; }

        public UILabel TitleLabel { get; private set; }

        public UIColor BorderColor
        {
            set
            {
                Layer.BorderColor = value.CGColor;
                SetNeedsDisplay();
            }
        }

        public TextViewWithDescription()
        {
            InitUi();
        }

        public TextViewWithDescription(NSCoder coder) : base(coder)
        {
            InitUi();
        }

        public TextViewWithDescription(NSObjectFlag t) : base(t)
        {
            InitUi();
        }

        internal TextViewWithDescription(IntPtr handle) : base(handle)
        {
            InitUi();
        }

        private void InitUi()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;
            TitleLabel = new UILabel
            {
                Font = UIFont.SystemFontOfSize(15),
                TextColor = AppColors.TrackSubtitleColor,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            TitleTextField = new UITextField
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(26, UIFontWeight.Bold),
                AdjustsFontSizeToFitWidth = true,
                MinimumFontSize = 18,
                TextColor = AppColors.TrackTitleColor,
            };
            TitleTextField.Started += TitleTextFieldOnEditingChanged;
            TitleTextField.Ended += TitleTextFieldOnEditingChanged;

            Layer.CornerRadius = 16;
            Layer.BorderWidth = 2;
            Layer.BorderColor = UIColor.Clear.CGColor;
            BackgroundColor = AppColors.StreakBackGroundColor;
            AddSubview(TitleLabel);
            AddSubview(TitleTextField);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                TrailingAnchor.ConstraintEqualTo(TitleLabel.TrailingAnchor, 16),
                TitleLabel.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, 16),
                TitleLabel.TopAnchor.ConstraintEqualTo(TopAnchor, 14),

                TitleTextField.TopAnchor.ConstraintEqualTo(TitleLabel.BottomAnchor, 4),
                TrailingAnchor.ConstraintEqualTo(TitleTextField.TrailingAnchor, 16),
                TitleTextField.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, 16),
                BottomAnchor.ConstraintEqualTo(TitleTextField.BottomAnchor, 14)
            });
        }

        private void TitleTextFieldOnEditingChanged(object sender, EventArgs e)
        {
            var textField = (UITextField)sender;
            if (textField.IsEditing)
            {
                BackgroundColor = UIColor.White;
                BorderColor = AppColors.ColorPrimary;
            }
            else
            {
                BackgroundColor = AppColors.StreakBackGroundColor;
                BorderColor = UIColor.Clear;
            }
        }
    }
}