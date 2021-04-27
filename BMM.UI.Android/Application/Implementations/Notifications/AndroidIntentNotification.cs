using System.Collections.Generic;
using Android.Content;
using BMM.Core.Implementations.Notifications;
using Newtonsoft.Json;

namespace BMM.UI.Droid.Application.Implementations.Notifications
{
    public class AndroidIntentNotification : IPlatformNotification
    {
        private readonly Intent _intent;

        public AndroidIntentNotification(Intent intent)
        {
            _intent = intent;
        }

        public string Title => "Android does not support reading this";

        public string Body => Title;

        public bool ContainsKey(string key)
        {
            return _intent.Extras.ContainsKey(key);
        }

        public string GetString(string key)
        {
            return _intent.Extras.GetString(key);
        }

        public IList<string> GetList(string key)
        {
            return JsonConvert.DeserializeObject<List<string>>(GetString(key));
        }

        public IList<int> GetIntList(string key)
        {
            return JsonConvert.DeserializeObject<List<int>>(GetString(key));
        }

        public int GetInt(string key)
        {
            // Even though it's sent as an int, we can only access it as string
            return int.Parse(GetString(key));
        }
    }
}