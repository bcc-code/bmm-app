using System.Globalization;
using BMM.Core.Constants;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Region.Interfaces;
using Newtonsoft.Json;

namespace BMM.Core.Implementations.Region
{
    public class CultureInfoRepository : ICultureInfoRepository
    {
        private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;
        private IList<CultureInfoLanguage> _cultureInfoLanguages;

        public CultureInfoRepository(IFirebaseRemoteConfig firebaseRemoteConfig)
        {
            _firebaseRemoteConfig = firebaseRemoteConfig;
        }

        public CultureInfoLanguage GetCultureInfoLanguage(CultureInfo cultureInfo) 
            => GetCultureInfoLanguage(cultureInfo.Name) ?? CreateDefaultCultureInfoLanguage(cultureInfo.Name);

        public CultureInfoLanguage GetCultureInfoLanguage(string name)
        {
            _cultureInfoLanguages ??= JsonConvert.DeserializeObject<IList<CultureInfoLanguage>>(_firebaseRemoteConfig.CultureInfoLanguages);
            var cultureInfoLanguage = _cultureInfoLanguages.FirstOrDefault(c => c.Name == name);
            return cultureInfoLanguage ?? CreateDefaultCultureInfoLanguage(name);
        }

        private CultureInfoLanguage CreateDefaultCultureInfoLanguage(string name)
        {
            return new CultureInfoLanguage(name, name, name);
        }
    }
}