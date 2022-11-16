using System.ComponentModel;

namespace BMM.Core.Models.POs.Other.Interfaces
{
    public interface ICheckboxListItemPO: IListContentItem, INotifyPropertyChanged
    {
        bool IsChecked { get; set; }
    }
}