using System;
using System.Collections.Generic;
using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Models;
using BMM.Core.ViewModels.Base;
using MvvmCross.ViewModels;
using MvvmCross;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Exceptions;
using MvvmCross.Base;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class LibraryArchiveViewModel : ItemListViewModel
    {
        private readonly IExceptionHandler _exceptionHandler;

        public MvxObservableCollection<ListItem> ListItems { get; }

        public LibraryArchiveViewModel(IExceptionHandler exceptionHandler)
            : base()
        {
            _exceptionHandler = exceptionHandler;
            ListItems = new MvxObservableCollection<ListItem>();
        }

        public override async Task Initialize()
        {
            try
            {
                await base.Initialize();
                await Load();
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
            }
        }

        public async Task Load()
        {
            await ReplaceDocuments(await GetListOfYears());
        }

        public virtual async Task<IList<DocumentYear>> GetListOfYears()
        {
           return await Client.Facets.GetAlbumPublishedYears();
        }

        private IMvxCommand<ListItem> _itemSelectedCommand;

        public IMvxCommand<ListItem> ItemSelectedCommand
        {
            get
            {
                _itemSelectedCommand = _itemSelectedCommand ?? new ExceptionHandlingCommand<ListItem>(DocumentYearAction);
                return _itemSelectedCommand;
            }
        }

        protected Task DocumentYearAction(ListItem item)
        {
            int year = int.Parse(item.Title);
            return NavigationService.Navigate<AlbumsViewModel, int>(year);
        }

        public virtual Task ReplaceDocuments(IEnumerable<DocumentYear> documents)
        {
            return Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>().ExecuteOnMainThreadAsync(() => {
                if (documents != null)
                {
                    var headerItems = documents.Reverse()
                        .Select(documentYear => new ListItem {Title = documentYear.Year.ToString()});

                    ListItems.ReplaceWith(headerItems);
                }
            });
        }
    }
}