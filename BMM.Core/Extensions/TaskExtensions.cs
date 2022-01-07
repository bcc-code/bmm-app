using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BMM.Core.Extensions
{
    public static class TaskExtensions
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static void FireAndForget(this Task task)
        {
        }
    }
}