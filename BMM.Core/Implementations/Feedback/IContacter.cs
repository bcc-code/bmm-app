using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BMM.Core.Implementations.Feedback
{
    public interface IContacter
    {
        /// <summary>
        ///   Provides a way to get in contact with support
        /// </summary>
        /// <exception cref="FeatureNotSupportedException">The current implementations way of sending support is not supported</exception>
        Task Contact();
    }
}