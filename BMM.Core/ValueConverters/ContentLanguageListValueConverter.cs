using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross.Converters;
using System;
using System.Globalization;
using System.Linq;
using MvvmCross.ViewModels;

namespace BMM.Core.ValueConverters
{
    public class ContentLanguageListValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((MvxObservableCollection<CultureInfo>)value).Select(l => new CellWrapperViewModel<CultureInfo>(l, (BaseViewModel)parameter));
        }
    }
}