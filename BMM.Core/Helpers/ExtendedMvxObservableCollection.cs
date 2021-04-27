using System.Collections.Generic;
using MvvmCross.ViewModels;

namespace BMM.Core.Helpers
{
    /// <summary>
    /// Optimized version of the <see cref="ExtendedMvxObservableCollection{T}"/> class.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class ExtendedMvxObservableCollection<T> : MvxObservableCollection<T>
    {

        public ExtendedMvxObservableCollection()
        {
                
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedMvxObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="items">The collection from which the items are copied.</param>
        public ExtendedMvxObservableCollection(IEnumerable<T> items)
            : base(items)
        {
           
        }

        public void RemoveAtWithoutTriggeringEvents(int position)
        {
            using (SuppressEvents())
            {
                RemoveAt(position);
            }
        }

        public void ClearWithoutTriggeringEvents()
        {
            using (SuppressEvents())
            {
                Clear();
            }
        }

        public void MoveWithoutTriggeringEvents(int fromPosition, int toPosition)
        {
            using (SuppressEvents())
            {
                Move(fromPosition, toPosition);
            }
        }
    }
}