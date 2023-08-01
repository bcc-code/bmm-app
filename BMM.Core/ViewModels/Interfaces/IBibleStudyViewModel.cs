using BMM.Api.Implementation.Models;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Interfaces;

public interface IBibleStudyViewModel : IBaseViewModel<IBibleStudyParameters>
{
    IBmmObservableCollection<IBasePO> Items { get; } 
}