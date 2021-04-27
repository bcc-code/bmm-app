using System.Collections.Generic;
using BMM.Core.Implementations.Notifications;
using Firebase.Messaging;
using Newtonsoft.Json;

namespace BMM.UI.Droid.Application.Implementations.Notifications
{
    public class RemoteMessageNotification : IPlatformNotification
    {
        private readonly RemoteMessage _message;
        private readonly RemoteMessage.Notification _notification;

        public RemoteMessageNotification(RemoteMessage message)
        {
            _message = message;
            _notification = _message.GetNotification();
        }

        public string Title => GetString("title");

        public string Body => GetString("body");

        public bool ContainsKey(string key)
        {
            return _message.Data.ContainsKey(key);
        }

        public string GetString(string key)
        {
            return _message.Data[key];
        }

        public IList<string> GetList(string key)
        {
            return JsonConvert.DeserializeObject<List<string>>(_message.Data[key]);
        }

        public IList<int> GetIntList(string key)
        {
            return JsonConvert.DeserializeObject<List<int>>(_message.Data[key]);
        }

        public int GetInt(string key)
        {
            return int.Parse(GetString(key));
        }
    }
}