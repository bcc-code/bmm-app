using CarPlay;

namespace BMM.UI.iOS.Extensions;

public static class CPListTemplateExtensions
{
    public static void SafeUpdateSections(this CPListTemplate template, CPListSection[] sections)
    {
        template.BeginInvokeOnMainThread(() => template.UpdateSections(sections));
    }
}