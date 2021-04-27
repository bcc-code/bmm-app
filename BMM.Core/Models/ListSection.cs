using System.Collections.Generic;

namespace BMM.Core.Models
{
    public class ListSection<TListItem>: IListSection<TListItem> where TListItem: IListContentItem
    {
        public string Title { get; set; }

        public IEnumerable<TListItem> Items { get; set; }
    }
}
