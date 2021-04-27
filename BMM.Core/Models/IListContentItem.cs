using MvvmCross.Commands;

namespace BMM.Core.Models
{
    public interface IListContentItem : IListItem
    {
        string Text { get; set; }
        IMvxCommand OnSelected { get; set; }
    }
}