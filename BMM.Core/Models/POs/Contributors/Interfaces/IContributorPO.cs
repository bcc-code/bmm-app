using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Contributors.Interfaces;

public interface IContributorPO : IDocumentPO
{
    IMvxAsyncCommand OptionButtonClickedCommand { get; }
    Contributor Contributor { get; }
}