using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.Test.Unit.GuardedActions.Base;
using BMM.Core.ViewModels.Base;
using FluentAssertions;
using MvvmCross.Base;
using MvvmCross.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.Documents
{
    [TestFixture]
    public class LoadMoreDocumentsActionTests : GuardedActionsTestBase<LoadMoreDocumentsAction>
    {
        private IPostprocessDocumentsAction _postprocessDocumentsActionMock;
        private IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;
        private ILoadMoreDocumentsViewModel _dataContextMock;
        private IBmmObservableCollection<IDocumentPO> _observableCollectionMock;

        public override void SetUp()
        {
            base.SetUp();
            GuardedAction.AttachDataContext(_dataContextMock);
        }

        protected override void PrepareMocks()
        {
            base.PrepareMocks();
            _postprocessDocumentsActionMock = Substitute.For<IPostprocessDocumentsAction>();
            _mvxMainThreadAsyncDispatcher = Substitute.For<IMvxMainThreadAsyncDispatcher>();
            _dataContextMock = Substitute.For<ILoadMoreDocumentsViewModel>();
            _observableCollectionMock = Substitute.For<IBmmObservableCollection<IDocumentPO>>();
            _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(Arg.Do<Action>(action => action()));

            _dataContextMock
                .Documents
                .Returns(_observableCollectionMock);
        }

        protected override LoadMoreDocumentsAction CreateAction()
        {
            return new(
                _postprocessDocumentsActionMock,
                _mvxMainThreadAsyncDispatcher);
        }

        [Test]
        public async Task DocumentsAreNotLoaded_WhenIsAlreadyLoading()
        {
            //Arrange
            _dataContextMock
                .IsLoading
                .Returns(true);

            //Act
            await GuardedAction.ExecuteGuarded();

            //Assert
            await _dataContextMock
                .DidNotReceive()
                .LoadItems(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CachePolicy>());
        }

        [Test]
        public async Task DocumentsAreNotLoaded_WhenIsFullyLoaded()
        {
            //Arrange
            _dataContextMock
                .IsFullyLoaded
                .Returns(true);

            //Act
            await GuardedAction.ExecuteGuarded();

            //Assert
            await _dataContextMock
                .DidNotReceive()
                .LoadItems(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CachePolicy>());
        }

        [Test]
        public async Task PostprocessDocumentsActionIsExecuted_WhenDocumentsAreLoaded()
        {
            //Arrange
            var documentsMock = Substitute.For<IEnumerable<IDocumentPO>>();

            _dataContextMock
                .LoadItems(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CachePolicy>())
                .Returns(documentsMock);

            //Act
            await GuardedAction.ExecuteGuarded();

            //Assert
            await _postprocessDocumentsActionMock
                .Received(1)
                .ExecuteGuarded(documentsMock);
        }

        [Test]
        public async Task CurrentLimitIsIncreasing_WhenDocumentsAreLoaded()
        {
            //Arrange && Act
            await GuardedAction.ExecuteGuarded();

            //Assert
            _dataContextMock
                .CurrentLimit
                .Should()
                .Be(ApiConstants.LoadMoreSize);
        }

        [Test]
        public async Task DocumentsArePopulatedOnDataContext_WhenProperlyRetrieved()
        {
            //Arrange
            var documents = new List<IDocumentPO>
            {
                Substitute.For<ITrackPO>()
            };

            _dataContextMock
                .LoadItems(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CachePolicy>())
                .Returns(documents);

            _postprocessDocumentsActionMock
                .ExecuteGuarded(documents)
                .Returns(documents);

            //Act
            await GuardedAction.ExecuteGuarded();

            //Assert
            _observableCollectionMock
                .AddRange(Arg.Is<List<DocumentPO>>(p => p.Count == 1));
        }
    }
}