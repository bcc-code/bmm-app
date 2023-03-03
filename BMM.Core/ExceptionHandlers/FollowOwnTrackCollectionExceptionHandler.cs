using Acr.UserDialogs;
using BMM.Api.Framework.Exceptions;
using BMM.Core.ExceptionHandlers.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;

namespace BMM.Core.ExceptionHandlers
{
    public class FollowOwnTrackCollectionExceptionHandler : IFollowOwnTrackCollectionExceptionHandler
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public FollowOwnTrackCollectionExceptionHandler(
            IUserDialogs userDialogs,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _userDialogs = userDialogs;
            _bmmLanguageBinder = bmmLanguageBinder;
        }

        public async Task HandleException(Exception ex)
        {
            await _userDialogs.AlertAsync(_bmmLanguageBinder[Translations.SharedTrackCollectionViewModel_FollowOwnTrackCollection]);
        }

        public IEnumerable<Type> GetTriggeringExceptionTypes()
        {
            yield return typeof(FollowOwnTrackCollectionException);
        }
    }
}