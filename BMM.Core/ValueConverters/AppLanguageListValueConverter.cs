using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MvvmCross.ViewModels;

namespace BMM.Core.ValueConverters
{
    /// <summary>
    /// This class helps to convert a list of languages to a list of languages, having a link to the ViewModel.
    /// This issue is described at https://github.com/MvvmCross/MvvmCross/issues/35
    /// </summary>
    public class AppLanguageListValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable items = value as IEnumerable;

            IList<LanguageCellWrapperViewModel> cellWrapperLanguages = items.OfType<CultureInfo>().Select(x => new LanguageCellWrapperViewModel(x, (BaseViewModel)parameter)).ToList();

            MvxObservableCollection<LanguageCellWrapperViewModel> languagesList = new MvxObservableCollection<LanguageCellWrapperViewModel>(cellWrapperLanguages);
            // Handling of collectionChanged event is not necessary, because list of languages is const and is never change.

            return languagesList;
        }
    }
}