namespace BMM.Core.Implementations.DeepLinking;

public class PendingDeepLink
{
    public PendingDeepLink(Uri uri, DeepLinkSource source)
    {
        Uri = uri;
        Source = source;
    }
    
    public Uri Uri { get; }
    public DeepLinkSource Source { get; }
}

public enum DeepLinkSource
{
    OutsideOfApp,
    InsideApp
}