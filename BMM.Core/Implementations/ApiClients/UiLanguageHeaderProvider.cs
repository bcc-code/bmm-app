using BMM.Api.Abstraction;
using BMM.Api.Implementation.Constants;
using BMM.Core.Implementations.Languages;

namespace BMM.Core.Implementations.ApiClients;

public class UiLanguageHeaderProvider : IHeaderProvider
{
    private readonly IAppLanguageProvider _appLanguageProvider;
    
    public UiLanguageHeaderProvider(IAppLanguageProvider appLanguageProvider)
    {
        _appLanguageProvider = appLanguageProvider;
    }
        
    public async Task<KeyValuePair<string, string>?> GetHeader()
    {
        return new KeyValuePair<string, string>(HeaderNames.UiLanguage, _appLanguageProvider.GetAppLanguage());
    }
}