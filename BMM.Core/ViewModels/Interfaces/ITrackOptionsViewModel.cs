using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface ITrackOptionsViewModel : IBaseViewModel<IOptionsListParameter>
    {
        public IBmmObservableCollection<StandardIconOptionPO> Options { get; }
        public MvxInteraction CloseInteraction { get; }
        public IMvxCommand CloseOptionsCommand { get; }
    }
}