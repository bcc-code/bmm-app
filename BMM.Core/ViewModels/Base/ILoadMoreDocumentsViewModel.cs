using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Base
{
    public interface ILoadMoreDocumentsViewModel : IBaseViewModel
    {
        IMvxAsyncCommand LoadMoreCommand { get; }

        bool IsFullyLoaded { get; set; }

        bool IsInitialized { get; }

        IBmmObservableCollection<IDocumentPO> Documents { get; }

        int CurrentLimit { get; set; }

        string TrackCountString { get; }

        Task<IEnumerable<IDocumentPO>> LoadItems(int startIndex, int size, CachePolicy policy);
    }
}