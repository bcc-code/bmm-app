using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BMM.Core.Helpers.Interfaces
{
    public interface IBmmObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        void AddRange(IEnumerable<T> items);

        void RemoveItems(IEnumerable<T> items);

        void ReplaceWith(IEnumerable<T> items);

        void ReplaceItem(int index, T item);

        void RefreshItem(T item);

        void InsertRange(int index, IEnumerable<T> items);

        void Move(int oldIndex, int newIndex);

        void SwitchTo(IEnumerable<T> items);

        void ReplaceRange(IEnumerable<T> items, int firstIndex, int oldSize);
    }
}