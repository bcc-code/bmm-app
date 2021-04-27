using MvvmCross.Commands;

namespace BMM.Core.Models
{
    public class SelectableListItem: ListItem, IListContentItem
    {
        public string Text { get; set; }
        public IMvxCommand OnSelected { get; set; }
    }
}