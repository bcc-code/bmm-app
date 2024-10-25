namespace BMM.UI.iOS.CustomViews;

public class CustomBadgeView : UIView
{
    UILabel _badgeLabel;

    public CustomBadgeView(string badgeText)
    {
        BackgroundColor = UIColor.Red;
        Layer.CornerRadius = 10;
        ClipsToBounds = true;

        _badgeLabel = new UILabel
        {
            Text = badgeText,
            TextColor = UIColor.White,
            TextAlignment = UITextAlignment.Center,
            Font = UIFont.SystemFontOfSize(14),  // Customize font size
        };

        AddSubview(_badgeLabel);
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        _badgeLabel.Frame = Bounds;
    }

    public void UpdateBadge(string text)
    {
        _badgeLabel.Text = text;
        SetNeedsLayout();
    }
}