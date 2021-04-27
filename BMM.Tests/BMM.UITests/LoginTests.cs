using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class LoginTests
    {
        private IBmmApp _bmmApp;
        private IApp _app;
        private readonly Platform _platform;

        public LoginTests(Platform platform)
        {
            _platform = platform;
        }

        [SetUp]
        public void Setup()
        {
            _bmmApp = AppInitializer.StartApp(_platform);
            _app = _bmmApp.App;
        }

        private string _(string methodName)
        {
            return methodName + (_platform == Platform.iOS ? ":" : "");
        }

        [Test]
        public async Task WhenValidCredentialsAreGiven_WillNavigateToExplorePage()
        {
           await _bmmApp.LoginToApp();
        }

        [Test]
        public async Task LoginLogoutLogin_WillNavigateToExplorePage()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenProfilePage(_app);
            Logout();

            _app.WaitForElement(_bmmApp.LoginPage.LoginButton);
            _app.Tap(_bmmApp.LoginPage.LoginButton);
            await _bmmApp.LoginToApp();
        }

        private void Logout()
        {
            _app.WaitForElement(_bmmApp.SettingsPage.Logout);
            _app.Tap(_bmmApp.SettingsPage.Logout);
            _app.Tap(_bmmApp.ConfirmOptionsPage.Ok);
        }
    }
}