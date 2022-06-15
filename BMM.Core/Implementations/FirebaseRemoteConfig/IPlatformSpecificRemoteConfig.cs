using System.Threading.Tasks;

namespace BMM.Core.Implementations.FirebaseRemoteConfig
{
    public interface IPlatformSpecificRemoteConfig
    {
        string GetStringValue(string id);

        bool GetBoolValue(string id);
        
        Task UpdateValuesFromFirebaseRemoteConfig();
    }
}