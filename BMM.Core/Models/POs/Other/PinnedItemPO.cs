using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.Other
{
    public class PinnedItemPO : DocumentPO
    {
        public PinnedItemPO(PinnedItem pinnedItem) : base(pinnedItem)
        {
            PinnedItem = pinnedItem;
        }
        
        public PinnedItem PinnedItem { get; }
    }
}