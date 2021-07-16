using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Security;
using Xamarin.Essentials;

namespace BMM.Core.Implementations.Feedback
{
    public class EmailContacter : IContacter
    {
        private readonly IUserStorage _userStorage;
        private readonly IDeviceInfo _deviceInfo;

        public EmailContacter(IUserStorage userStorage, IDeviceInfo deviceInfo)
        {
            _userStorage = userStorage;
            _deviceInfo = deviceInfo;
        }

        public async Task Contact()
        {
            var user = _userStorage.GetUser();
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine("App version: " + GlobalConstants.AppVersion);
            sb.AppendLine("Manufacturer: " + _deviceInfo.Manufacturer);
            sb.AppendLine("Device model: " + _deviceInfo.Model);
            sb.AppendLine("Device platform: " + _deviceInfo.Platform);
            sb.AppendLine("Device version: " + _deviceInfo.VersionString);
            sb.AppendLine("Full name: " + user.FullName);
            sb.AppendLine("Username: " + user.Username);
            sb.AppendLine("PersonId: " + user.PersonId);

            var email = new EmailMessage
            {
                To = new List<string> {GlobalConstants.ContactMailAddress},
                Subject = "BMM App",
                Body = sb.ToString(),
            };

            await Email.ComposeAsync(email);
        }
    }
}
