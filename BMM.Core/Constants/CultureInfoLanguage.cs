namespace BMM.Core.Constants;

public class CultureInfoLanguage
{
    public const string DefaultCultureInfoLanguagesJson =
        "[{\"Code\":\"af\",\"EnglishName\":\"Afrikaans\",\"NativeName\":\"Afrikaans\"}," +
        "{\"Code\":\"bg\",\"EnglishName\":\"Bulgarian\", \"NativeName\":\"български\"}," +
        "{\"Code\":\"cs\",\"EnglishName\":\"Czech\",\"NativeName\":\"čeština\"}," +
        "{\"Code\":\"da\", \"EnglishName\":\"Danish\",\"NativeName\":\"dansk\"}" +
        ",{\"Code\":\"de\",\"EnglishName\":\"German\",\"NativeName\":\"Deutsch\"}," +
        "{\"Code\":\"en\",\"EnglishName\":\"English\",\"NativeName\":\"English\"}," +
        "{\"Code\":\"el\",\"EnglishName\":\"Greek\", \"NativeName\":\"Ελληνικά\"}," +
        "{\"Code\":\"es\",\"EnglishName\":\"Spanish\",\"NativeName\":\"español\"}," +
        "{\"Code\":\"et\", \"EnglishName\":\"Estonian\",\"NativeName\":\"eesti\"}," +
        "{\"Code\":\"fr\",\"EnglishName\":\"French\",\"NativeName\":\"Français\"}," +
        "{\"Code\":\"fi\",\"EnglishName\":\"Finnish\",\"NativeName\":\"suomi\"}," +
        "{\"Code\":\"he\",\"EnglishName\":\"Hebrew\",\"NativeName\":\"עברית\"}," +
        "{\"Code\":\"hr\",\"EnglishName\":\"Croatian\",\"NativeName\":\"hrvatski\"}," +
        "{\"Code\":\"hu\", \"EnglishName\":\"Hungarian\",\"NativeName\":\"magyar\"}," +
        "{\"Code\":\"it\",\"EnglishName\":\"Italian\",\"NativeName\":\"italiano\"}," +
        "{\"Code\":\"kha\",\"EnglishName\":\"Khasi\",\"NativeName\":\"Khasi\"}," +
        "{\"Code\":\"nb\",\"EnglishName\":\"Norwegian Bokmål\",\"NativeName\":\"norsk bokmål\"}," +
        "{\"Code\":\"nl\",\"EnglishName\":\"Dutch\",\"NativeName\":\"Nederlands\"}," +
        "{\"Code\":\"ml\",\"EnglishName\":\"Malayalam\",\"NativeName\":\"മലയ\u0d3eള\u0d02\"}," +
        "{\"Code\":\"pl\",\"EnglishName\":\"Polish\",\"NativeName\":\"polski\"}," +
        "{\"Code\":\"pt\",\"EnglishName\":\"Portuguese\",\"NativeName\":\"português\"}," +
        "{\"Code\":\"ro\",\"EnglishName\":\"Romanian\",\"NativeName\":\"română\"}," +
        "{\"Code\":\"ru\",\"EnglishName\":\"Russian\",\"NativeName\":\"русский\"}," +
        "{\"Code\":\"sl\",\"EnglishName\":\"Slovenian\",\"NativeName\":\"slovenščina\"}," +
        "{\"Code\":\"ta\",\"EnglishName\":\"Tamil\",\"NativeName\":\"தம\u0bbfழ\u0bcd\"}," +
        "{\"Code\":\"tr\",\"EnglishName\":\"Turkish\",\"NativeName\":\"Türkçe\"}," +
        "{\"Code\":\"zh\",\"EnglishName\":\"Chinese (Simplified)\",\"NativeName\":\"中文\"}," +
        "{\"Code\":\"uk\",\"EnglishName\":\"Ukrainian\",\"NativeName\":\"українська\"}," +
        "{\"Code\":\"yue\",\"EnglishName\":\"Cantonese\",\"NativeName\":\"廣東話\"}]";

    public CultureInfoLanguage(string code, string englishName, string nativeName)
    {
        Code = code;
        EnglishName = englishName;
        NativeName = nativeName;
    }

    public string Code { get; }
    public string EnglishName { get; }
    public string NativeName { get; }
}