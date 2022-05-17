using System.Threading.Tasks;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface ISiriShortcutsViewModel : IBaseViewModel
    {
        IBmmObservableCollection<StandardSelectablePO> AvailableShortcuts { get; }
        IMvxCommand<StandardSelectablePO> ShortcutSelectedCommand { get; }
        Task Refresh();
    }
}