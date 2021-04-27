using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Implementations.UI;
using MvvmCross.Localization;
using Xamarin.Essentials;

namespace BMM.Core.Implementations.Feedback
{
    public class ErrorCatchingContacterDecorator : IContacter
    {
        private readonly IContacter _contacter;

        private readonly IToastDisplayer _toastDisplayer;

        public IMvxLanguageBinder GlobalTextSource => new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Global");

        public ErrorCatchingContacterDecorator(IContacter contacter, IToastDisplayer toastDisplayer)
        {
            _contacter = contacter;
            _toastDisplayer = toastDisplayer;
        }

        public async Task Contact()
        {
            try
            {
                await _contacter.Contact();
            }
            catch (FeatureNotSupportedException)
            {
                _toastDisplayer.Warn(GlobalTextSource.GetText("ContactError", GlobalConstants.ContactMailAddress));
            }
        }
    }
}
