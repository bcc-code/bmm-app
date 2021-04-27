namespace BMM.Core.Helpers
{
    public class OidcConstants
    {
        public const string AuthorizationServerUrl = "https://login.bcc.no";
        public const string ClientId = "atNnW7N113LOEFnTBOLpZvXueLknm4uE";
        public const string Scopes = "openid profile email offline_access";
        public const string LoginRedirectUrl = "org.brunstad.bmm://login-callback";
        public const string Audience = "https://bmm-api.brunstad.org";
        public const string PromptParameter = "login";
        public const string OidcMinimumAppVersionRequired = "1.23.0";
        public const string OidcMinimumAndroidVersionRequired = "5.0";
    }
}