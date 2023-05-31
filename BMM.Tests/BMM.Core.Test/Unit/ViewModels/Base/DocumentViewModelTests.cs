using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Test.Helpers;
using Moq;
using MvvmCross.Base;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels.Base
{
    public class DocumentViewModelTests : BaseViewModelTests
    {
        protected DocumentsViewModelImplementation DocumentsViewModel { get; set; }
        protected Mock<IBMMClient> Client { get; set; }

        protected override void AdditionalSetup()
        {
            base.AdditionalSetup();
            Client = new Mock<IBMMClient>();

            Ioc.RegisterSingleton(Client.Object);
            Ioc.RegisterSingleton(new Mock<INotificationCenter>(MockBehavior.Strict).Object);
            Ioc.RegisterSingleton(new Mock<IListenedTracksStorage>().Object);
            Ioc.RegisterSingleton(new Mock<IMediaQueue>().Object);
            Ioc.RegisterSingleton(new Mock<IMediaPlayer>().Object);
            Ioc.RegisterSingleton(new Mock<IConnection>().Object);

            DocumentsViewModel = new DocumentsViewModelImplementation();
            DocumentsViewModel.PostprocessDocumentsAction = new Mock<IPostprocessDocumentsAction>().Object;
            DocumentsViewModel.MvxMainThreadAsyncDispatcher = MockDispatcher;
        }

        [Test]
        public void Reload_SetsIsRefreshingToTrue()
        {
            Assert.False(DocumentsViewModel.IsRefreshing);

            DocumentsViewModel.LoadItemsAction = () =>
            {
                //Simulate load process
                Thread.Sleep(1000);
                return new List<IDocumentPO>();
            };

            DocumentsViewModel.Refresh();

            Assert.True(DocumentsViewModel.IsRefreshing);
        }

        [Test]
        public void LoadItem_SetsIsLoadingToTrue()
        {
            Assert.False(DocumentsViewModel.IsLoading);

            DocumentsViewModel.LoadItemsAction = () =>
            {
                //Simulate load process
                Thread.Sleep(1000);
                return new List<IDocumentPO>();
            };
            DocumentsViewModel.Load();

            Assert.True(DocumentsViewModel.IsLoading);
        }

        [Test]
        public async Task LoadItem_ErrorEmitted_PassesExceptionToTheOutside()
        {
            // Arrange
            DocumentsViewModel.LoadItemsAction = () => throw new NoInternetException();

            // Act & Assert
            Assert.ThrowsAsync<NoInternetException>(() => DocumentsViewModel.Load());
        }
        
        [Test]
        public async Task Reload_Completed_NotReloading()
        {
            DocumentsViewModel.LoadItemsAction = () => new List<IDocumentPO>();
            await DocumentsViewModel.Refresh();

            Assert.False(DocumentsViewModel.IsRefreshing);
        }

        [Test]
        public async Task LoadItem_Completed_NotLoading()
        {
            DocumentsViewModel.LoadItemsAction = () => new List<IDocumentPO>();
            await DocumentsViewModel.Load();

            Assert.False(DocumentsViewModel.IsLoading);
        }
    }
}