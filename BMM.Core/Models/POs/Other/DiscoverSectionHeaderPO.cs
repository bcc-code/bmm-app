using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.DeepLinks.Interfaces;
using BMM.Core.GuardedActions.DeepLinks.Parameters;
using BMM.Core.Models.POs.Base;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Other
{
    public class DiscoverSectionHeaderPO : DocumentPO
    {
        public DiscoverSectionHeaderPO(
            IInternalDeepLinkAction internalDeepLinkAction,
            DiscoverSectionHeader discoverSectionHeader) : base(discoverSectionHeader)
        {
            DiscoverSectionHeader = discoverSectionHeader;
            DeepLinkButtonClickedCommand = new MvxCommand(() =>
            {
                internalDeepLinkAction.ExecuteGuarded(new InternalDeepLinkActionParameter(DiscoverSectionHeader.Link, DiscoverSectionHeader.Origin));
            });
        }
        
        public DiscoverSectionHeader DiscoverSectionHeader { get; }
        public IMvxCommand DeepLinkButtonClickedCommand { get; }
    }
}