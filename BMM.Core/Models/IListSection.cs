using System.Collections.Generic;

namespace BMM.Core.Models
{
    public interface IListSection<TListItem>: IListItem where TListItem: IListContentItem
    {
        IEnumerable<TListItem> Items { get; set; }
    }
}