using BMM.Api.Implementation.Models;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Interfaces;

public interface IBibleStudyRulesViewModel : IBaseViewModel
{
    IBmmObservableCollection<IBasePO> Items { get; }
    string Title { get; set; }
}