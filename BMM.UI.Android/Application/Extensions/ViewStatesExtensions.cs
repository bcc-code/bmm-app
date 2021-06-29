using Android.Views;

namespace BMM.UI.Droid.Application.Extensions
{
    public static class ViewStatesExtensions
    {
        public static ViewStates ToViewState(this bool isVisible)
        {
            return isVisible
                ? ViewStates.Visible
                : ViewStates.Gone;
        }
    }
}