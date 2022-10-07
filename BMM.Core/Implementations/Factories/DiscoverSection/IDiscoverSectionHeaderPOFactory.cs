using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Other;

namespace BMM.Core.Implementations.Factories.DiscoverSection
{
    public interface IDiscoverSectionHeaderPOFactory
    {
        DiscoverSectionHeaderPO Create(DiscoverSectionHeader discoverSectionHeader);
    }
}