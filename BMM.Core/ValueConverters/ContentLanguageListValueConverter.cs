using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross.Converters;
using System;
using System.Globalization;
using System.Linq;
using BMM.Core.Constants;
using MvvmCross.ViewModels;

namespace BMM.Core.ValueConverters
{
    public class ContentLanguageListValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((MvxObservableCollection<CultureInfoLanguage>)value).Select(l => new CellWrapperViewModel<CultureInfoLanguage>(l, (BaseViewModel)parameter));
        }
    }
}