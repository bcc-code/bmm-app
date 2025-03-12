namespace BMM.UI.iOS.Extensions;

public static class DictionaryExtensions
{
    public static NSDictionary ToNSDictionary(this IDictionary<string, string> dictionary)
    {
        if (dictionary == null)
            return new NSDictionary();
        
        var keys = new NSString[dictionary.Count];
        var values = new NSString[dictionary.Count];
        int index = 0;

        foreach (var kvp in dictionary)
        {
            keys[index] = new NSString(kvp.Key);
            values[index] = new NSString(kvp.Value);
            index++;
        }

        return NSDictionary.FromObjectsAndKeys(values, keys);
    }
}