using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BMM.Core.Implementations.Region.Interfaces;
using BMM.Core.Models.Regions;

namespace BMM.Core.Implementations.Region
{
    public class CultureInfoRepository : ICultureInfoRepository
    {
        private const string UnknownCultureEnglishName = "Unknown language";
        private IList<CultureInfo> _customCultureInfos;

        public CultureInfoRepository()
        {
            CreateCustomCultureInfos();
        }

        public CultureInfo Get(string iso)
        {
            try
            {
                return new CultureInfo(iso);
            }
            catch (CultureNotFoundException)
            {
                return GetCustomCultureInfoFor(iso);
            }
        }

        private CultureInfo GetCustomCultureInfoFor(string isoCode)
        {
            var customCulture = _customCultureInfos
                .FirstOrDefault(c => c.TwoLetterISOLanguageName == isoCode);

            if (customCulture != null)
                return customCulture;

            return new CustomCultureInfo(isoCode,
                isoCode,
                UnknownCultureEnglishName,
                string.Empty);
        }

        private void CreateCustomCultureInfos()
        {
            _customCultureInfos = new List<CultureInfo>()
            {
                new CustomCultureInfo("yue", "廣東話", "Cantonese", "zh")
            };
        }
    }
}