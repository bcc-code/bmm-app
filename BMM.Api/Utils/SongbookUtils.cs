namespace BMM.Api.Utils
{
    public static class SongbookUtils
    {
        public static string GetShortName(string name)
        {
            if (name == "herrens_veier")
                return "HV";
            if (name == "mandelblomsten")
                return "FMB";
            return name;
        }
    }
}