namespace BMM.Core.Utils;

public static class LoggingUtils
{
    /// <summary>
    ///     This splits up a long string into multiple parameters.
    /// </summary>
    public static void AddParameter(this Dictionary<string, object> paramsDictionary,
        string name,
        string message,
        int maxLength,
        int maxParts)
    {
        int partNumber = 1;
        
        for (int i = 0; i < message.Length; i += maxLength)
        {
            if (maxLength + i > message.Length)
                maxLength = message.Length - i;

            paramsDictionary.Add(GetName(name, partNumber), message.Substring(i, maxLength));
            
            if (paramsDictionary.Count == maxParts)
                break;
            
            partNumber++;
        }
    }

    private static string GetName(string name, int partNumber)
    {
        return partNumber == 1
            ? name
            : $"{name}_{partNumber}";
    }
}