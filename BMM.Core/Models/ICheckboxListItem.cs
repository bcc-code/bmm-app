using System.ComponentModel;

namespace BMM.Core.Models
{
    public interface ICheckboxListItem: IListContentItem, INotifyPropertyChanged
    {
        bool IsChecked { get; set; }
    }
}