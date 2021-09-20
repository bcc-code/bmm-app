using System;
using BMM.Core.GuardedActions.Navigation;
using BMM.Core.GuardedActions.Navigation.Interfaces;
using BMM.Core.Test.Unit.GuardedActions.Base;
using BMM.Core.ViewModels;
using MvvmCross.Navigation;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.Navigation
{
    [TestFixture]
    public class NavigateToViewModelActionTests : GuardedActionWithParameterTestBase<INavigateToViewModelAction, Type>
    {
        private IMvxNavigationService _mvxNavigationServiceMock;

        protected override void PrepareMocks()
        {
            base.PrepareMocks();
            _mvxNavigationServiceMock = Substitute.For<IMvxNavigationService>();
        }

        protected override INavigateToViewModelAction CreateAction()
        {
            return new NavigateToViewModelAction(_mvxNavigationServiceMock);
        }

        [Test]
        public void NavigationToSpecifiedVMIsPerformed_WhenActionIsExecuted()
        {
            //Arrange
            var vmType = typeof(ExploreNewestViewModel);

            //Act
            GuardedAction.ExecuteGuarded(vmType);

            //Assert
            _mvxNavigationServiceMock
                .Received(1)
                .Navigate(vmType);
        }
    }
}