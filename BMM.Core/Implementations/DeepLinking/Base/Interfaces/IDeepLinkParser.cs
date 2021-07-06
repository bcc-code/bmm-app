using System;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.DeepLinking.Base.Interfaces
{
    public interface IDeepLinkParser
    {
        bool PerformCanNavigateTo(Uri uri, out Func<Task> action);
    }
}