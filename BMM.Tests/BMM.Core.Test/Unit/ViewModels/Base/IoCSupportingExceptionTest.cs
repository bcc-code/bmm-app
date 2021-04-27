using System;
using BMM.Core.Implementations.Exceptions;
using Moq;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.Tests;

namespace BMM.Core.Test.Unit.ViewModels.Base
{
    public abstract class IoCSupportingExceptionTest: MvxIoCSupportingTest
    {
        private Mock<IExceptionHandler> _exceptionHandlerMock;
        protected IExceptionHandler ExceptionHandler;

        protected override void AdditionalSetup()
        {
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
            ExceptionHandler = _exceptionHandlerMock.Object;

            Mvx.IoCProvider.RegisterSingleton(ExceptionHandler);

            Mvx.IoCProvider.RegisterType(() => new Mock<IMvxMessenger>().Object);
            Mvx.IoCProvider.RegisterType(() => new Mock<IMvxNavigationService>().Object);

            base.AdditionalSetup();
        }

        protected void AssertExceptionsHandled(int numberOfExceptions)
        {
            _exceptionHandlerMock
                .Verify(exHandler =>
                        exHandler.HandleException(It.IsAny<Exception>()),Times.Exactly(numberOfExceptions));
        }
    }
}