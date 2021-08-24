using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Helpers.Interfaces;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.ViewModels;

namespace BMM.Core.Helpers
{
    public class BmmObservableCollection<T> : MvxObservableCollection<T>, IBmmObservableCollection<T>
    {
        public BmmObservableCollection()
        { }

        public BmmObservableCollection(IEnumerable<T> items)
            : base(items)
        { }

        public void ReplaceItem(int index, T item)
        {
            using (SuppressEvents())
            {
                this[index] = item;
            }

            RefreshItem(item);
        }

        public void RefreshItem(T item)
        {
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, item));
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            using (SuppressEvents())
            {
                var currentIndex = index;
                foreach (var item in items)
                {
                    InsertItem(currentIndex++, item);
                }
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                changedItems: items.ToList(),
                index));
        }
    }
}