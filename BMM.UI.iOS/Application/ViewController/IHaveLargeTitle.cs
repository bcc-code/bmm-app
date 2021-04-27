namespace BMM.UI.iOS
{
    /// <summary>
    /// Calling 'NavigationItem.LargeTitleDisplayMode' multiple times causes some visual bugs.
    /// Because of that we introduces this interface to set this setting once inside of <see cref="BaseViewController{TViewModel}"/>.
    /// This is done instead of overriding it in an inheritor.
    /// </summary>
    public interface IHaveLargeTitle
    {
        double? InitialLargeTitleHeight { get; set; }
    }
}