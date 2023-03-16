namespace BMM.Core.Support;

public interface IOldSecureStorage
{
    Task<string> GetAsync(string key);
    Task SetAsync(string key, string value);
    bool Remove(string key);
    void RemoveAll();
}