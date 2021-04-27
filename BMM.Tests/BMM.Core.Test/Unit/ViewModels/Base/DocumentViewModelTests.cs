using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Test.Helpers;
using Moq;
using MvvmCross.Base;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels.Base
{
    public class DocumentViewModelTests: IoCSupportingExceptionTest
    {
        protected DocumentsViewModelImplementation DocumentsViewModel { get; set; }
        protected Mock<IBMMClient> Client { get; set; }

        protected override void AdditionalSetup()
        {
            base.AdditionalSetup();
            Client = new Mock<IBMMClient>();

            var mockDispatcher = new MockDispatcher();
            Ioc.RegisterSingleton(Client.Object);
            Ioc.RegisterSingleton<IMvxMainThreadDispatcher>(mockDispatcher);
            Ioc.RegisterSingleton<IMvxMainThreadAsyncDispatcher>(mockDispatcher);
            Ioc.RegisterSingleton(new Mock<INotificationCenter>(MockBehavior.Strict).Object);
            Ioc.RegisterSingleton(new Mock<IListenedTracksStorage>().Object);
            Ioc.RegisterSingleton(new Mock<IMediaQueue>().Object);
            Ioc.RegisterSingleton(new Mock<IMediaPlayer>().Object);
            Ioc.RegisterSingleton(new Mock<IConnection>().Object);

            DocumentsViewModel = new DocumentsViewModelImplementation();
        }

        [Test]
        public void Reload_SetsIsRefreshingToTrue()
        {
            base.Setup();

            Assert.False(DocumentsViewModel.IsRefreshing);

            DocumentsViewModel.LoadItemsAction = () =>
            {
                //Simulate load process
                Thread.Sleep(1000);
                return new List<Document>();
            };

            DocumentsViewModel.Refresh();

            Assert.True(DocumentsViewModel.IsRefreshing);
        }

        [Test]
        public void LoadItem_SetsIsLoadingToTrue()
        {
            base.Setup();

            Assert.False(DocumentsViewModel.IsLoading);

            DocumentsViewModel.LoadItemsAction = () =>
            {
                //Simulate load process
                Thread.Sleep(1000);
                return new List<Document>();
            };
            DocumentsViewModel.Load();

            Assert.True(DocumentsViewModel.IsLoading);
        }

        [Test]
        public async Task LoadItem_ErrorEmitted_PassesExceptionToTheOutside()
        {
            // Arrange
            Setup();
            DocumentsViewModel.LoadItemsAction = () => throw new NoInternetException();

            // Act & Assert
            Assert.ThrowsAsync<NoInternetException>(() => DocumentsViewModel.Load());
        }


        [Test]
        public async Task Reload_Completed_NotReloading()
        {
            base.Setup();

            DocumentsViewModel.LoadItemsAction = () => new List<Document>();
            await DocumentsViewModel.Refresh();

            Assert.False(DocumentsViewModel.IsRefreshing);
        }

        [Test]
        public async Task LoadItem_Completed_NotLoading()
        {
            base.Setup();

            DocumentsViewModel.LoadItemsAction = () => new List<Document>();
            await DocumentsViewModel.Load();

            Assert.False(DocumentsViewModel.IsLoading);
        }
    }
}