using BMM.Api;
using BMM.Api.Framework;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.NewMediaPlayer.Abstractions;
using Moq;
using MvvmCross.Base;
using MvvmCross.Navigation;
using MvvmCross.Localization;
using MvvmCross.Plugin.Messenger;
using MvvmCross.Tests;

namespace BMM.Core.Test.Unit.ViewModels.Base
{
    public class BaseViewModelTests : MvxIoCSupportingTest
    {
        protected override void AdditionalSetup()
        {
            Client = new Mock<IBMMClient>();
            MediaQueue = new Mock<IMediaQueue>();
            ExceptionHandler = new Mock<IExceptionHandler>();
            TextResource = new Mock<IMvxLanguageBinder>();
            Analytics = new Mock<IAnalytics>();
            MvxMessenger = new Mock<IMvxMessenger>();
            NavigationService = new Mock<IMvxNavigationService>();

            Ioc.RegisterSingleton(Client.Object);
            Ioc.RegisterSingleton(MediaQueue.Object);
            Ioc.RegisterSingleton(ExceptionHandler.Object);
            Ioc.RegisterSingleton(MvxMessenger.Object);
            Ioc.RegisterSingleton(NavigationService.Object);
            Ioc.RegisterSingleton(new Mock<IMvxTextProvider>().Object);
            Ioc.RegisterSingleton(new Mock<INotificationCenter>(MockBehavior.Strict).Object);
            Ioc.RegisterSingleton(new Mock<IMediaPlayer>().Object);
            Ioc.RegisterSingleton(new Mock<IMvxMainThreadDispatcher>().Object);
            Ioc.RegisterSingleton(new Mock<IConnection>().Object);
        }

        protected Mock<IAnalytics> Analytics { get; private set; }

        protected Mock<IBMMClient> Client { get; private set; }

        protected Mock<IMediaQueue> MediaQueue { get; private set; }

        protected Mock<IExceptionHandler> ExceptionHandler { get; private set; }

        protected Mock<IMvxLanguageBinder> TextResource { get; private set; }

        protected Mock<IMvxMessenger> MvxMessenger { get; private set; }

        protected Mock<IMvxNavigationService> NavigationService { get; private set; }
    }
}
