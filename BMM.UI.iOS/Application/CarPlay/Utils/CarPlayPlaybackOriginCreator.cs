using BMM.UI.iOS.CarPlay.Creators.Base;

namespace BMM.UI.iOS.CarPlay.Utils;

public static class CarPlayPlaybackOriginCreator
{
    private const string Prefix = "CarPlay";
    private const string SuffixToTrim = "LayoutCreator";

    public static string CreatePlaybackOrigin(this BaseLayoutCreator layoutCreator)
    {
        string fullName = layoutCreator.GetType().Name;
        
        string name = fullName.EndsWith(SuffixToTrim)
            ? fullName.Substring(0, fullName.Length - SuffixToTrim.Length)
            : fullName;
        
        return $"{Prefix}_{name}";
    }
}