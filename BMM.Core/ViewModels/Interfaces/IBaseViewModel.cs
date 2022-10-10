using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface IBaseViewModel : IMvxViewModel, IMvxNotifyPropertyChanged
    {
        IBMMLanguageBinder TextSource { get; }
        IMvxAsyncCommand CloseCommand { get; }
        IMvxCommand<IDocumentPO> DocumentSelectedCommand { get; }
        IMvxAsyncCommand<Document> OptionCommand { get; }
        bool IsLoading { get; set; }
        string PlaybackOriginString { get; }
    }
}