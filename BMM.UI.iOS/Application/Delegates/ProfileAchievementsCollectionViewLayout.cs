namespace BMM.UI.iOS.Delegates;

public class ProfileAchievementsCollectionViewLayout : UICollectionViewFlowLayout
{
    public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
    {
        var attributes = base.LayoutAttributesForElementsInRect(rect);

        nfloat leftMargin = SectionInset.Left;
        float maxY = -1.0f;

        foreach (var layoutAttribute in attributes)
        {
            if (layoutAttribute.Frame.GetMinY() >= maxY)
                leftMargin = SectionInset.Left;

            layoutAttribute.Frame = new CGRect(leftMargin, layoutAttribute.Frame.GetMinY(), layoutAttribute.Frame.Width, layoutAttribute.Frame.Height);

            leftMargin += layoutAttribute.Frame.Width + MinimumInteritemSpacing;
            maxY = Math.Max((float)layoutAttribute.Frame.GetMaxY(), maxY);
        }

        return attributes;
    }
}