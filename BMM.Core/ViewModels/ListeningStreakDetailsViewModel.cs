using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class ListeningStreakDetailsViewModel : BaseViewModel<IListeningStreakDetailsParameter>
    {
        public ListeningStreakDetailsViewModel()
        {
            CloseOptionsCommand = new MvxCommand(() => CloseInteraction.Raise());
            OptionSelectedCommand = new MvxAsyncCommand<StandardIconOptionPO>(async option =>
            {
                await CloseCommand.ExecuteAsync();
                option.ClickCommand.Execute();
            });
        }

        public IBmmObservableCollection<StandardIconOptionPO> Options { get; } = new BmmObservableCollection<StandardIconOptionPO>();
        public MvxInteraction CloseInteraction { get; } = new();
        public IMvxCommand CloseOptionsCommand { get; }
        public IMvxAsyncCommand<StandardIconOptionPO> OptionSelectedCommand { get; }
        public ListeningStreak ListeningStreak { get; private set; }

        public override async Task Initialize()
        {
            await base.Initialize();
            ListeningStreak = NavigationParameter.ListeningStreak;
        }
    }
}