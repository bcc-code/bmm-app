using BMM.Api.Implementation.Models;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IDocumentsViewModel : IBaseViewModel
    {
        public IBmmObservableCollection<IDocumentPO> Documents { get; }
    }
}