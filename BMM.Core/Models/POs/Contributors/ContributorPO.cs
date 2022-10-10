using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Contributors
{
    public class ContributorPO : DocumentPO
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