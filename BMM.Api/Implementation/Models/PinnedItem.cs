using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Models
{
    public class PinnedItem : Document
    {
        public string Title { get; set; }

        public string Icon { get; set; }

        public object Action { get; set; }

        public PinnedItemActionType ActionType { get; set; }
        
        public PinnedItem()
        {
            DocumentType = DocumentType.PinnedItem;
        }
    }
}