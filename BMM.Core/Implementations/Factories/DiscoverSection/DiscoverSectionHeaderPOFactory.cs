using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.DeepLinks.Interfaces;
using BMM.Core.Models.POs.Other;

namespace BMM.Core.Implementations.Factories.DiscoverSection
{
    public class DiscoverSectionHeaderPOFactory : IDiscoverSectionHeaderPOFactory
    {
        private readonly IInternalDeepLinkAction _internalDeepLinkAction;

        public DiscoverSectionHeaderPOFactory(IInternalDeepLinkAction internalDeepLinkAction)
        {
            _internalDeepLinkAction = internalDeepLinkAction;
        }
        
        public DiscoverSectionHeaderPO Create(DiscoverSectionHeader discoverSectionHeader)
        {
            return new DiscoverSectionHeaderPO(_internalDeepLinkAction, discoverSectionHeader);
        }
    }
}