using BMM.Api.Implementation.Models;
using BMM.Core.Helpers.Interfaces;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IDocumentsViewModel : IBaseViewModel
    {
        public IBmmObservableCollection<Document> Documents { get; }
    }
}