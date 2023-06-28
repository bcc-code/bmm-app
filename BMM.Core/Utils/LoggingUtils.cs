namespace BMM.Core.Utils;

public static class LoggingUtils
{
    public static void AddParameter(this Dictionary<string, object> paramsDictionary,
        string name,
        string message,
        int maxLength)
    {
        int partNumber = 1;
        
        for (int i = 0; i < message.Length; i += maxLength)
        {
            if (maxLength + i > message.Length)
                maxLength = message.Length - i;

            paramsDictionary.Add(GetName(name, partNumber), message.Substring(i, maxLength));
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