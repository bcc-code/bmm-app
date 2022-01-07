using BMM.Api.Abstraction;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IChangeTrackLanguageViewModel : IBaseViewModel<ITrackModel>
    {
        IBmmObservableCollection<BasePO> AvailableLanguages { get; }
    }
}