using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.Implementations.Tracks.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Navigation;

namespace BMM.UI.iOS.Implementations.Track
{
    public class iOSTrackOptionsService : ITrackOptionsService
    {
        private readonly IMvxNavigationService _mvxNavigationService;

        public iOSTrackOptionsService(IMvxNavigationService mvxNavigationService) 
        {
            _mvxNavigationService = mvxNavigationService;
        }

        public Task OpenOptions(IList<StandardIconOptionPO> optionsList)
        {
            _mvxNavigationService.Navigate<OptionsListViewModel, IOptionsListParameter>(new OptionsListParameter(optionsList));
            return Task.CompletedTask;
        }
    }
}