using System.Threading.Tasks;

namespace BMM.Core.Implementations.Startup
{
    public interface IDelayedStartupTask
    {
        /// <summary>
        /// Method is run a few seconds after the startup of the app.
        /// Caution: The user might not be logged in at this point.
        /// </summary>
        Task RunAfterStartup();
    }
}