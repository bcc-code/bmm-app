namespace BMM.Core.GuardedActions.DeepLinks.Parameters
{
    public class InternalDeepLinkActionParameter
    {
        public InternalDeepLinkActionParameter(string link, string origin)
        {
            Link = link;
            Origin = origin;
        }
        
        public string Link { get; }
        public string Origin { get; }
    }
}