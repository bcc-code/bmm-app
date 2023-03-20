using System.Globalization;
using BMM.Core.Constants;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class LanguageNameValueConverter : MvxValueConverter<CultureInfoLanguage, string>
    {
        protected override string Convert(CultureInfoLanguage value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{FirstCharToUpper(value.NativeName)} ({FirstCharToUpper(value.EnglishName)})";
        }

        private string FirstCharToUpper(string input)
        {
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}