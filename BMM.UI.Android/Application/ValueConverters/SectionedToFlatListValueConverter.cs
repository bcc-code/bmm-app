using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BMM.Core.Models;
using MvvmCross.Converters;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class SectionedToFlatListValueConverter :
        MvxValueConverter<IEnumerable<ListSection<IListContentItem>>, IEnumerable<IListItem>>
    {
        protected override IEnumerable<IListItem> Convert(IEnumerable<ListSection<IListContentItem>> value, Type targetType, object parameter, CultureInfo culture)
        {
            var sections = value;

            return sections.SelectMany(section =>
            {
                if (string.IsNullOrEmpty(section.Title))
                    return section.Items;

                var header = new SectionHeader
                {
                    Title = section.Title
                };

                return new List<IListItem> {header}.Concat(section.Items);
            });
        }
    }
}