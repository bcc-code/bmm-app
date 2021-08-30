using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.ExceptionHandlers.Interfaces;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Tracklist;
using BMM.Core.Messages;
using BMM.Core.Test.Unit.GuardedActions.Base;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.Tracklist
{
    [TestFixture]
    public class AddToMyPlaylistActionTests : GuardedActionsTestBase<AddToMyPlaylistAction>
    {
        private const string TestCollectionName = nameof(TestCollectionName);
        private const int TestCollectionId = 1;
        private const string TestSharingSecret = nameof(TestSharingSecret);

        private IBMMClient _bmmClientMock;
        private IMvxMessenger _mvxMessengerMock;
        private IMvxNavigationService _mvxNavigationServiceMock;
        private IFollowOwnTrackCollectionExceptionHandler _followOwnTrackCollectionExceptionHandlerMock;
        private ISharedTrackCollectionViewModel _dataContextMock;
        private ISharedPlaylistClient _sharedPlaylistClientMock;
        private IMvxAsyncCommand _closeCommandMock;

        public override void SetUp()
        {
            base.SetUp();
            GuardedAction.AttachDataContext(_dataContextMock);
        }

        protected override void PrepareMocks()
        {
            base.PrepareMocks();
            _bmmClientMock = Substitute.For<IBMMClient>();
            _mvxMessengerMock = Substitute.For<IMvxMessenger>();
            _mvxNavigationServiceMock = Substitute.For<IMvxNavigationService>();
            _followOwnTrackCollectionExceptionHandlerMock = Substitute.For<IFollowOwnTrackCollectionExceptionHandler>();
            _dataContextMock = Substitute.For<ISharedTrackCollectionViewModel>();
            _sharedPlaylistClientMock = Substitute.For<ISharedPlaylistClient>();
            _closeCommandMock = Substitute.For<IMvxAsyncCommand>();

            _bmmClientMock
                .SharedPlaylist
                .Returns(_sharedPlaylistClientMock);

            var myCollectionMock = Substitute.For<TrackCollection>();

            myCollectionMock.Id = TestCollectionId;
            myCollectionMock.Name = TestCollectionName;

            _dataContextMock
                .SharingSecret
                .Returns(TestSharingSecret);

            _dataContextMock
                .MyCollection
                .Returns(myCollectionMock);

            _dataContextMock
                .CloseCommand
                .Returns(_closeCommandMock);
        }

        protected override AddToMyPlaylistAction CreateAction()
        {
            return new(
                _bmmClientMock,
                _mvxMessengerMock,
                _mvxNavigationServiceMock,
                _followOwnTrackCollectionExceptionHandlerMock);
        }

        [Test]
        public async Task FollowRequestIsSent()
        {
            //Arrange && Act
            await GuardedAction.ExecuteGuarded();

            //Assert
            await _sharedPlaylistClientMock
                .Received(1)
                .Follow(TestSharingSecret);
        }

        [Test]
        public async Task PlaylistStateChangedMessageIsSent()
        {
            //Arrange && Act
            await GuardedAction.ExecuteGuarded();

            //Assert
            _mvxMessengerMock
                .Received(1)
                .Publish(Arg.Is<PlaylistStateChangedMessage>(m => m.Id == TestCollectionId));
        }

        [Test]
        public async Task CloseCommandIsExecuted()
        {
            //Arrange && Act
            await GuardedAction.ExecuteGuarded();

            //Assert
            await _closeCommandMock
                .Received(1)
                .ExecuteAsync();
        }

        [Test]
        public async Task NavigationTrackCollectionViewModelIsExecuted()
        {
            //Arrange && Act
            await GuardedAction.ExecuteGuarded();

            //Assert
            await _mvxNavigationServiceMock
                .Received(1)
                .Navigate<TrackCollectionViewModel, ITrackCollectionParameter>(
                    Arg.Is<ITrackCollectionParameter>(p =>
                        p.Name == TestCollectionName
                        && p.TrackCollectionId == TestCollectionId));
        }
    }
}