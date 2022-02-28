using AndroidX.ConstraintLayout.Widget;

namespace BMM.UI.Droid.Application.Extensions
{
    public static class BoolExtensions
    {
        public static int GetConstraintVisibility(this bool isVisible)
        {
            return isVisible
                ? ConstraintSet.Visible
                : ConstraintSet.Gone;
        }
    }
}