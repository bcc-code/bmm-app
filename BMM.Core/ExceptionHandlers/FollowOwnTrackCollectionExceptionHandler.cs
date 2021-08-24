using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Framework.Exceptions;
using BMM.Core.ExceptionHandlers.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using MvvmCross.Localization;

namespace BMM.Core.ExceptionHandlers
{
    public class FollowOwnTrackCollectionExceptionHandler : IFollowOwnTrackCollectionExceptionHandler
    {
        private readonly MvxLanguageBinder _textSource;
        private readonly IUserDialogs _userDialogs;

        public FollowOwnTrackCollectionExceptionHandler(IUserDialogs userDialogs)
        {
            _userDialogs = userDialogs;
            _textSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, nameof(SharedTrackCollectionViewModel));
        }

        public async Task HandleException(Exception ex)
        {
            await _userDialogs.AlertAsync(_textSource.GetText("FollowOwnTrackCollection"));
        }

        public IEnumerable<Type> GetTriggeringExceptionTypes()
        {
            yield return typeof(FollowOwnTrackCollectionException);
        }
    }
}