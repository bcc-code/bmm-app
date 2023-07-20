using BMM.Api.Implementation.Models;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.ViewModels.Interfaces;

public interface IBibleStudyViewModel : IBaseViewModel
{
    IBmmObservableCollection<IBasePO> Items { get; } 
}