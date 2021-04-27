namespace BMM.Core.Implementations
{
    public interface IViewModelAwareViewPresenter
    {
        /// <summary>
        /// Must be called form the main thread
        /// </summary>
        bool IsViewModelShown<T>();

        /// <summary>
        /// Must be called form the main thread
        /// </summary>
        bool IsViewModelInStack<T>();
    }
}
