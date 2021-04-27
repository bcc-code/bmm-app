using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface ILoginPage
    {
        Func<AppQuery, AppWebQuery> WebUserName { get; }
        Func<AppQuery, AppWebQuery> WebPassword { get; }
        Func<AppQuery, AppWebQuery> WebLoginButton { get; }
        Func<AppQuery, AppQuery> LoginButton { get; }
        Func<AppQuery, AppQuery> WebView { get; }
        Func<AppQuery, AppQuery> AlertWindow { get; }
        Func<AppQuery, AppQuery> AlertWindowCloseButton { get; }
        Func<AppQuery, AppQuery> NoConnectionAlert { get; }
        Func<AppQuery, AppQuery> AlertWindowLogInError { get; }

    }

    public abstract class LoginPage : ILoginPage
    {
        public abstract Func<AppQuery, AppWebQuery> WebUserName { get; }
        public abstract Func<AppQuery, AppWebQuery> WebPassword { get; }
        public abstract Func<AppQuery, AppWebQuery> WebLoginButton { get; }
        public abstract Func<AppQuery, AppQuery> WebView { get; }

        public abstract Func<AppQuery, AppQuery> LoginButton { get; }
        public abstract Func<AppQuery, AppQuery> AlertWindow { get; }
        public abstract Func<AppQuery, AppQuery> AlertWindowCloseButton { get; }

        public Func<AppQuery, AppQuery> NoConnectionAlert
        {
            get
            {
                return c => c.Text("There is currently no connection available. Please try again later");
            }
        }

        public Func<AppQuery, AppQuery> AlertWindowLogInError
        {
            get
            {
                return c => c.Text("Please provide a valid email address. Using the username for login is no longer supported.");
            }
        }
    }

    public class AndroidLoginPage : LoginPage
    {
        public override Func<AppQuery, AppWebQuery> WebUserName
        {
            get
            {
                return c => c.Css("input[type=\"email\"]");
            }
        }

        public override Func<AppQuery, AppWebQuery> WebPassword
        {
            get
            {
                return c => c.Css("input[type=\"password\"]");
            }
        }

        public override Func<AppQuery, AppWebQuery> WebLoginButton
        {
            get
            {
                return c => c.Css("button[type=\"submit\"]");
            }
        }

        public override Func<AppQuery, AppQuery> WebView
        {
            get
            {
                return c => c.WebView();
            }
        }

        public override Func<AppQuery, AppQuery> LoginButton
        {
            get
            {
                return c => c.Id("BtnLogin");
            }
        }

        public override Func<AppQuery, AppQuery> AlertWindow
        {
            get
            {
                return c => c.Id("alertTitle");
            }
        }

        public override Func<AppQuery, AppQuery> AlertWindowCloseButton
        {
            get
            {
                return c => c.Id("button1");
            }
        }

    }

    public class TouchLoginPage : LoginPage
    {
        public override Func<AppQuery, AppWebQuery> WebUserName
        {
            get
            {
                return c => c.Class("WKWebView").Css("input[type=\"email\"]");
            }
        }

        public override Func<AppQuery, AppWebQuery> WebPassword
        {
            get
            {
                return c => c.Class("WKWebView").Css("input[type=\"password\"]");
            }
        }

        public override Func<AppQuery, AppWebQuery> WebLoginButton
        {
            get
            {
                return c => c.Class("WKWebView").Css("button[type=\"submit\"]");
            }
        }

        public override Func<AppQuery, AppQuery> WebView
        {
            get
            {
                return c => c.Class("WKWebView");
            }
        }

        public override Func<AppQuery, AppQuery> LoginButton
        {
            get
            {
                return c => c.Id("LoginButton");
            }
        }

        public override Func<AppQuery, AppQuery> AlertWindow
        {
            get
            {
                return c => c.Class("UITransitionView").Child().Index(2);
            }
        }

        public override Func<AppQuery, AppQuery> AlertWindowCloseButton
        {
            get
            {
                return c => c.Text("Ok");
            }
        }
    }
}