using MvvmCross.Core;
using MvvmCross.ViewModels;
using Newtonsoft.Json;

namespace BMM.Core.Extensions
{
    public static class MvxBundleExtensions
    {
        private const string ParameterKey = "Parameter";

        public static void SaveParameter<T>(this IMvxBundle mvxBundle, T parameter)
        {
            mvxBundle.Data.Add(ParameterKey, JsonConvert.SerializeObject(parameter));
        }

        public static T RetrieveParameter<T>(this IMvxBundle state)
        {
            string data = null;
            state.SafeGetData()?.TryGetValue(ParameterKey, out data);

            if (data == null)
                return default;

            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}