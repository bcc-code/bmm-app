using System.Globalization;
using BMM.Core.Constants;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Region.Interfaces;
using Newtonsoft.Json;

namespace BMM.Core.Implementations.Region
{
    public class CultureInfoRepository : ICultureInfoRepository
    {
        private const string UnknownCultureEnglishName = "Unknown language";
        private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;
        private IList<CultureInfoLanguage> _cultureInfoLanguages;

        public CultureInfoRepository(IFirebaseRemoteConfig firebaseRemoteConfig)
        {
            _firebaseRemoteConfig = firebaseRemoteConfig;
        }

        public CultureInfoLanguage GetCultureInfoLanguage(string code)
        {
            _cultureInfoLanguages ??= JsonConvert.DeserializeObject<IList<CultureInfoLanguage>>(_firebaseRemoteConfig.CultureInfoLanguages);
            var cultureInfoLanguage = _cultureInfoLanguages.FirstOrDefault(c => c.Code == code);
            return cultureInfoLanguage ?? CreateUnknownCultureInfoLanguage(code);
        }

        private CultureInfoLanguage CreateUnknownCultureInfoLanguage(string code)
        {
            return new CultureInfoLanguage(code, UnknownCultureEnglishName, code);
        }
    }
}