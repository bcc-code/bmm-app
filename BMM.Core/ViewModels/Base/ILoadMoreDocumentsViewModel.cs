using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Base
{
    public interface ILoadMoreDocumentsViewModel : IMvxViewModel, IMvxNotifyPropertyChanged
    {
        IMvxAsyncCommand LoadMoreCommand { get; }

        bool IsLoading { get; set; }

        bool IsFullyLoaded { get; set; }

        bool IsInitialized { get; }

        MvxObservableCollection<Document> Documents { get; }

        int CurrentLimit { get; set; }

        string TrackCountString { get; }

        Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy);
    }
}