using System.ComponentModel;

namespace BMM.Core.Models.POs.Other.Interfaces
{
    public interface ICheckboxListItemPO: IListContentItem, INotifyPropertyChanged
    {
        string Key { get; set; }
        bool IsChecked { get; set; }
        bool IsEnabled { get; set; }
    }
}