using BMM.Core.Implementations.FeatureToggles;
using MvvmCross.IoC;

namespace BMM.Core.Helpers
{
    public static class IocProviderHelper
    {
        public static void RegisterTypeIfMissing<TFrom, TTo>(this IMvxIoCProvider ioCProvider)
            where TFrom : class
            where TTo : class, TFrom
        {
            if (!ioCProvider.CanResolve<TFrom>())
                ioCProvider.RegisterType<TFrom, TTo>();
        }

        public static void RegisterDecorator<TInterface, TDecorator, TImplementation>(this IMvxIoCProvider ioCProvider) where TInterface : class where TDecorator : class, TInterface where TImplementation : class, TInterface
        {
            ioCProvider.RegisterType<TInterface>(() =>
            {
                var childContainer = ioCProvider.CreateChildContainer();
                childContainer.RegisterType<TInterface, TImplementation>();
                childContainer.RegisterType<TDecorator, TDecorator>();
                return childContainer.Resolve<TDecorator>();
            });
        }

        public static void RegisterDecorator<TInterface, TDecorator1, TDecorator2, TImplementation>(this IMvxIoCProvider ioCProvider) where TInterface : class where TDecorator1 : class, TInterface where TDecorator2 : class, TInterface where TImplementation : class, TInterface
        {
            var tempContainer = ioCProvider.CreateChildContainer();

            tempContainer.RegisterType<TInterface>(() =>
            {
                var childContainer = ioCProvider.CreateChildContainer();
                childContainer.RegisterType<TInterface, TImplementation>();
                childContainer.RegisterType<TDecorator2, TDecorator2>();
                return childContainer.Resolve<TDecorator2>();
            });

            ioCProvider.RegisterType<TInterface>(() =>
            {
                var childContainer = tempContainer.CreateChildContainer();
                childContainer.RegisterType<TDecorator1, TDecorator1>();
                return childContainer.Resolve<TDecorator1>();
            });
        }

        public static void RegisterSingletonDecorator<TInterface, TDecorator, TImplementation>(this IMvxIoCProvider ioCProvider) where TInterface : class where TDecorator : class, TInterface where TImplementation : class, TInterface
        {
            ioCProvider.RegisterDecorator<TInterface, TDecorator, TImplementation>();
            ioCProvider.RegisterSingleton<TInterface>(ioCProvider.Resolve<TInterface>());
        }

        public static void RegisterSingletonIfFeatureToggled<TInterface, TToggle, TImplementation, TFallback>(this IMvxIoCProvider ioCProvider)
            where TInterface : class
            where TToggle : class, IFeatureToggle
            where TImplementation : class, TInterface
            where TFallback : class, TInterface
        {
            var toggle = ioCProvider.Resolve<TToggle>();
            if (toggle.IsEnabled())
            {
                ioCProvider.LazyConstructAndRegisterSingleton<TInterface, TImplementation>();
            }
            else
            {
               ioCProvider.LazyConstructAndRegisterSingleton<TInterface, TFallback>();
            }
        }

        public static void ConstructAndRegisterSingletonIfNotRegistered<T, TImplementation>(this IMvxIoCProvider ioCProvider) where T : class where TImplementation : class, T
        {
            if (!ioCProvider.CanResolve<T>())
            {
                ioCProvider.ConstructAndRegisterSingleton<T, TImplementation>();
            }
        }
    }
}