namespace BMM.Core.Constants;

public class CultureInfoLanguage
{
    public const string DefaultCultureInfoLanguagesJson =
        "[{\"Name\":\"af\",\"EnglishName\":\"Afrikaans\",\"NativeName\":\"Afrikaans\"}," +
        "{\"Name\":\"bg\",\"EnglishName\":\"Bulgarian\", \"NativeName\":\"български\"}," +
        "{\"Name\":\"cs\",\"EnglishName\":\"Czech\",\"NativeName\":\"čeština\"}," +
        "{\"Name\":\"da\", \"EnglishName\":\"Danish\",\"NativeName\":\"dansk\"}" +
        ",{\"Name\":\"de\",\"EnglishName\":\"German\",\"NativeName\":\"Deutsch\"}," +
        "{\"Name\":\"en\",\"EnglishName\":\"English\",\"NativeName\":\"English\"}," +
        "{\"Name\":\"el\",\"EnglishName\":\"Greek\", \"NativeName\":\"Ελληνικά\"}," +
        "{\"Name\":\"es\",\"EnglishName\":\"Spanish\",\"NativeName\":\"español\"}," +
        "{\"Name\":\"et\", \"EnglishName\":\"Estonian\",\"NativeName\":\"eesti\"}," +
        "{\"Name\":\"fr\",\"EnglishName\":\"French\",\"NativeName\":\"Français\"}," +
        "{\"Name\":\"fi\",\"EnglishName\":\"Finnish\",\"NativeName\":\"suomi\"}," +
        "{\"Name\":\"he\",\"EnglishName\":\"Hebrew\",\"NativeName\":\"עברית\"}," +
        "{\"Name\":\"hr\",\"EnglishName\":\"Croatian\",\"NativeName\":\"hrvatski\"}," +
        "{\"Name\":\"hu\", \"EnglishName\":\"Hungarian\",\"NativeName\":\"magyar\"}," +
        "{\"Name\":\"it\",\"EnglishName\":\"Italian\",\"NativeName\":\"italiano\"}," +
        "{\"Name\":\"kha\",\"EnglishName\":\"Khasi\",\"NativeName\":\"Khasi\"}," +
        "{\"Name\":\"nb\",\"EnglishName\":\"Norwegian Bokmål\",\"NativeName\":\"norsk bokmål\"}," +
        "{\"Name\":\"nl\",\"EnglishName\":\"Dutch\",\"NativeName\":\"Nederlands\"}," +
        "{\"Name\":\"ml\",\"EnglishName\":\"Malayalam\",\"NativeName\":\"മലയ\u0d3eള\u0d02\"}," +
        "{\"Name\":\"pl\",\"EnglishName\":\"Polish\",\"NativeName\":\"polski\"}," +
        "{\"Name\":\"pt\",\"EnglishName\":\"Portuguese\",\"NativeName\":\"português\"}," +
        "{\"Name\":\"ro\",\"EnglishName\":\"Romanian\",\"NativeName\":\"română\"}," +
        "{\"Name\":\"ru\",\"EnglishName\":\"Russian\",\"NativeName\":\"русский\"}," +
        "{\"Name\":\"sl\",\"EnglishName\":\"Slovenian\",\"NativeName\":\"slovenščina\"}," +
        "{\"Name\":\"ta\",\"EnglishName\":\"Tamil\",\"NativeName\":\"தம\u0bbfழ\u0bcd\"}," +
        "{\"Name\":\"tr\",\"EnglishName\":\"Turkish\",\"NativeName\":\"Türkçe\"}," +
        "{\"Name\":\"zh\",\"EnglishName\":\"Chinese (Simplified)\",\"NativeName\":\"中文\"}," +
        "{\"Name\":\"uk\",\"EnglishName\":\"Ukrainian\",\"NativeName\":\"українська\"}," +
        "{\"Name\":\"yue\",\"EnglishName\":\"Cantonese\",\"NativeName\":\"廣東話\"}]";

    public CultureInfoLanguage(string name, string englishName, string nativeName)
    {
        Name = name;
        EnglishName = englishName;
        NativeName = nativeName;
    }

    public string Name { get; }
    public string EnglishName { get; }
    public string NativeName { get; }
}