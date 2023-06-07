using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Utils;
using MvvmCross;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Startup
{
    interface IStartupManager
    {
        void Initialize(IEnumerable<Type> creatableTypes);
    }

    public class StartupManager : IStartupManager
    {
        public const int StartupDelayInSeconds = 5;

        private readonly IExceptionHandler _exceptionHandler;
        private readonly IMvxMessenger _messenger;

        private IEnumerable<Type> _creatableTypes;
        private readonly DebounceDispatcher _debouncer;

        public StartupManager(
            IExceptionHandler exceptionHandler,
            IMvxMessenger messenger)
        {
            _exceptionHandler = exceptionHandler;
            _messenger = messenger;
            _debouncer = new DebounceDispatcher(StartupDelayInSeconds.ToMilliseconds());
        }

        public void Initialize(IEnumerable<Type> creatableTypes)
        {
            _creatableTypes = creatableTypes;

            _messenger.Subscribe((LoggedInMessage message) =>
                {
                    var typesToRunAtStartup = _creatableTypes.Where(x => x.GetInterfaces().Contains(typeof(INeedInitialization))).ToList();
                    foreach (var type in typesToRunAtStartup)
                    {
                        var instance = CreateInstance<INeedInitialization>(type);
                        _exceptionHandler.FireAndForgetWithoutUserMessages(async () => await instance.InitializeWhenLoggedIn());
                    }
                },
                MvxReference.Strong);

            _debouncer.Run(DetectAndRunStartupTasks);
        }

        /// <summary>
        /// fyi: on Android this methods runs for ~30ms
        /// </summary>
        private void DetectAndRunStartupTasks()
        {
            var startupTaskInterfaceType = typeof(IDelayedStartupTask);
            var typesToRunAtStartup = _creatableTypes.Where(x => x.GetInterfaces().Contains(startupTaskInterfaceType)).ToList();

            foreach (var type in typesToRunAtStartup)
            {
                var instance = CreateInstance<IDelayedStartupTask>(type);
                _exceptionHandler.FireAndForgetWithoutUserMessages(async () => await instance.RunAfterStartup());
            }
        }

        /// <summary>
        /// Create an instance for the <see cref="IDelayedStartupTask"/>.
        /// Often only the interface is registered but not the type itself. (e.g. <see cref="ListenAnalytics"/>) Therefore we also check the interfaces.
        /// </summary>
        private T CreateInstance<T>(Type type) where T : class
        {
            if (Mvx.IoCProvider.CanResolve(type))
            {
                return Mvx.IoCProvider.Resolve(type) as T;
            }

            // Often only the interface is registered but not the type itself
            var interfaces = type.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (Mvx.IoCProvider.CanResolve(@interface))
                {
                    return Mvx.IoCProvider.Resolve(@interface) as T;
                }
            }

            throw new Exception("unable to create an instance of " + type.FullName);
        }
    }
}
