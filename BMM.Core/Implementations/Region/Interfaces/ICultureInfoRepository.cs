using System.Globalization;

namespace BMM.Core.Implementations.Region.Interfaces
{
    public interface ICultureInfoRepository
    {
        CultureInfo Get(string iso);
    }
}