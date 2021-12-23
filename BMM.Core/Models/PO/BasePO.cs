using MvvmCross.ViewModels;

namespace BMM.Core.Models.PO
{
    /// <summary>
    /// Base Presentation Object class.
    /// It can be used for models that have direct bindings to view, but are not View Models e.g list items, custom view
    /// </summary>
    public abstract class BasePO : MvxNotifyPropertyChanged
    {
    }
}