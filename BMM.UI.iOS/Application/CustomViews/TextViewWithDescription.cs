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

        internal TextViewWithDescription(ObjCRuntime.NativeHandle handle) : base(handle)
        {
            InitUi();
        }

        private void InitUi()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;
            TitleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            TitleLabel.ApplyTextTheme(AppTheme.Subtitle2Label2);

            TitleTextField = new UITextField
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = UIFont.SystemFontOfSize(26, UIFontWeight.Bold),
                AdjustsFontSizeToFitWidth = true,
            };

            TitleTextField.Font = Typography.Header3.Value;
            TitleTextField.TextColor = AppColors.LabelOneColor;

            TitleTextField.Started += TitleTextFieldOnEditingChanged;
            TitleTextField.Ended += TitleTextFieldOnEditingChanged;

            Layer.CornerRadius = 16;
            Layer.BorderWidth = 2;
            Layer.BorderColor = UIColor.Clear.CGColor;
            BackgroundColor = AppColors.BackgroundTwoColor;
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
                BackgroundColor = AppColors.BackgroundOneColor;
                BorderColor = AppColors.TintColor;
            }
            else
            {
                BackgroundColor = AppColors.BackgroundTwoColor;
                BorderColor = UIColor.Clear;
            }
        }
    }
}