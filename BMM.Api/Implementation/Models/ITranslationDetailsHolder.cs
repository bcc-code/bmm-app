namespace BMM.Api.Implementation.Models
{
    public interface ITranslationDetailsHolder
    {
        string TranslationParent { get; set; }
        string TranslationId { get; set; }
    }
}