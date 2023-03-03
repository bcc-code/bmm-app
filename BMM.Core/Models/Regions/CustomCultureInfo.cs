using System.Globalization;
using System.Reflection;

namespace BMM.Core.Models.Regions
{
    public class CustomCultureInfo : CultureInfo
    {
        public CustomCultureInfo(string isoCode,
            string nativeName,
            string englishName,
            string baseCultureName) : base(baseCultureName, false)
        {
            TwoLetterISOLanguageName = isoCode;
            Name = isoCode;
            NativeName = nativeName;
            EnglishName = englishName;
            SetNameField(isoCode);
        }
        
        public override string TwoLetterISOLanguageName { get; }
        public override string Name { get; }
        public override string NativeName { get; }
        public override string EnglishName { get; }
        
        // 'm_name' field has to bo set in order to correctly comparing two CultureInfo’’’
        private void SetNameField(string isoCode)
        {
            var fields = typeof(CultureInfo).GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            var nameField = fields.First(x => x.Name == "_name");
            nameField.SetValue(this, isoCode);
        }
    }
}