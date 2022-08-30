namespace BMM.Core.Implementations.Languages
{
    public class LanguageDescription
    {
        public string NativeName { get; set; }
        public string EnglishName { get; set; }
        public string FullDescription => $"{FirstCharToUpper(NativeName)} ({FirstCharToUpper(EnglishName)})";
        
        private string FirstCharToUpper(string input)
        {
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}