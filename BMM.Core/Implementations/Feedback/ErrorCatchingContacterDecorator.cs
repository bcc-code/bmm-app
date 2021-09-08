using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Translation;
using MvvmCross.Localization;
using Xamarin.Essentials;

namespace BMM.Core.Implementations.Feedback
{
    public class ErrorCatchingContacterDecorator : IContacter
    {
        private readonly IContacter _contacter;
        private readonly IToastDisplayer _toastDisplayer;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public ErrorCatchingContacterDecorator(
            IContacter contacter,
            IToastDisplayer toastDisplayer,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _contacter = contacter;
            _toastDisplayer = toastDisplayer;
            _bmmLanguageBinder = bmmLanguageBinder;
        }

        public async Task Contact()
        {
            try
            {
                await _contacter.Contact();
            }
            catch (FeatureNotSupportedException)
            {
                _toastDisplayer.Warn(_bmmLanguageBinder.GetText(Translations.Global_ContactError, GlobalConstants.ContactMailAddress));
            }
        }
    }
}
