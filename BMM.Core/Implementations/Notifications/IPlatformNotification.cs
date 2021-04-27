using System.Collections.Generic;

namespace BMM.Core.Implementations.Notifications
{
    public interface IPlatformNotification
    {
        bool ContainsKey(string key);

        string GetString(string key);

        string Title {get; }

        string Body { get; }

        IList<string> GetList(string key);

        IList<int> GetIntList(string key);

        int GetInt(string key);
    }
}