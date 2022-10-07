using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using MvvmCross.ViewModels;

namespace BMM.Core.Models.POs.Base
{
    /// <summary>
    /// Base Presentation Object class.
    /// It can be used for models that have direct bindings to view, but are not View Models e.g list items, custom view
    /// </summary>
    public abstract class BasePO : MvxNotifyPropertyChanged
    {
        public IBMMLanguageBinder TextSource => BMMLanguageBinderLocator.TextSource;
    }
}