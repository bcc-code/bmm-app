using System;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.DeepLinking.Base
{
    public abstract class DeepLinkParserBase
    {
        public bool PerformCanNavigateTo(Uri uri, out Func<Task> action)
        {
            try
            {
                return CanNavigateTo(uri, out action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                action = null;
                return false;
            }
        }

        protected abstract bool CanNavigateTo(Uri uri, out Func<Task> action);
    }
}