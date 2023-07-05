using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Contributors.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Contributors
{
    public class ContributorPO : DocumentPO, IContributorPO
    {
        public ContributorPO(
            IMvxAsyncCommand<Document> optionsClickedCommand,
            Contributor contributor) : base(contributor)
        {
            OptionButtonClickedCommand = new MvxAsyncCommand(async () =>
            {
                await optionsClickedCommand.ExecuteAsync(Contributor);
            });
            
            Contributor = contributor;
        }
        
        public IMvxAsyncCommand OptionButtonClickedCommand { get; }
        public Contributor Contributor { get; }
    }
}