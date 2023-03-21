using System.Globalization;
using BMM.Core.Constants;

namespace BMM.Core.Implementations.Region.Interfaces
{
    public interface ICultureInfoRepository
    {
        CultureInfoLanguage GetCultureInfoLanguage(string code);
    }
}