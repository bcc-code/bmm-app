using System.Collections.Generic;
using BMM.Core.Implementations.Notifications;
using Foundation;
using Newtonsoft.Json;
using UserNotifications;

namespace BMM.UI.iOS.Implementations.Notifications
{
    public class IosNotification : IPlatformNotification
    {
        private readonly NSDictionary _message;
        private readonly UNNotification _notification;

        public string Title => _notification.Request.Content.Title;

        public string Body => _notification.Request.Content.Body;

        public IosNotification(UNNotification notification)
        {
            _notification = notification;
            _message = notification.Request.Content.UserInfo;
        }

        public IosNotification(NSDictionary userInfo)
        {
            _message = userInfo;
        }

        public bool ContainsKey(string key)
        {
            return _message.ContainsKey((NSString)key);
        }

        public string GetString(string key)
        {
            return (NSString)_message[key];
        }

        public int GetInt(string key)
        {
            var value = int.Parse(_message[key] as NSString);
            return value;
        }

        public IList<string> GetList(string key)
        {
            var listAsString = _message[key] as NSString;
            var list = JsonConvert.DeserializeObject<List<string>>(listAsString);
            return list;
        }

        public IList<int> GetIntList(string key)
        {
            var listAsString = _message[key] as NSString;
            var list = JsonConvert.DeserializeObject<List<int>>(listAsString);
            return list;
        }
    }
}