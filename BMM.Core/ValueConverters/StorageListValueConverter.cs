using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StorageListValueConverter : MvxValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable items = value as IEnumerable;
            IList<StorageCellWrapperViewModel> cellWrapperStorage = items.OfType<IFileStorage>().Select(x => new StorageCellWrapperViewModel(x, (BaseViewModel)parameter)).ToList();
            return cellWrapperStorage;
        }
    }
}
