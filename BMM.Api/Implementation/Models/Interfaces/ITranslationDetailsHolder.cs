namespace BMM.Api.Implementation.Models.Interfaces
{
    public interface ITranslationDetailsHolder
    {
        string TranslationParent { get; set; }
        string TranslationId { get; set; }
    }
}