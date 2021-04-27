namespace BMM.UI.Droid.Application.Helpers
{
    public interface ISdkVersionHelper
    {
        bool SupportsNavigationBarColors { get; }

        bool SupportsNavigationBarDividerColor { get; }

        bool HasProblemsWithSslHandshakes { get; }
    }
}