using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface ISettingsPage
    {
        Func<AppQuery, AppQuery> SettingsList { get; }
        Func<AppQuery, AppQuery> DownloadViaMobileNetworkSwitch { get; }
        Func<AppQuery, AppQuery> Logout { get; }
        Func<AppQuery, AppQuery> ContentLanguage { get; }
        Func<AppQuery, AppQuery> AppLanguage { get; }
    }

    public class AndroidSettingsPage : ISettingsPage
    {
        public Func<AppQuery, AppQuery> SettingsList
        {
            get
            {
                return c => c.Id("settings_list");
            }
        }

        public Func<AppQuery, AppQuery> DownloadViaMobileNetworkSwitch
        {
            get
            {
                return c => SettingsList(c).Descendant().Marked("Download via mobile network");
            }
        }

        public Func<AppQuery, AppQuery> ContentLanguage
        {
            get
            {
                return c => c.Marked("Content language");
            }
        }

        public Func<AppQuery, AppQuery> AppLanguage
        {
            get
            {
                return c => SettingsList(c).Descendant().Marked("App language");
            }
        }

        public Func<AppQuery, AppQuery> Logout
        {
            get
            {
                return c => c.Marked("Log out");
            }
        }
    }

    public class TouchSettingsPage : ISettingsPage
    {
        public Func<AppQuery, AppQuery> SettingsList
        {
            get
            {
                return c => c.Id("settings_table");
            }
        }

        public Func<AppQuery, AppQuery> DownloadViaMobileNetworkSwitch
        {
            get
            {
                return c => c.Text("Download via mobile network");
            }
        }

        public Func<AppQuery, AppQuery> ContentLanguage
        {
            get
            {
                return c => c.Text("Content language");
            }
        }

        public Func<AppQuery, AppQuery> AppLanguage
        {
            get
            {
                return c => c.Text("App language");
            }
        }

        public Func<AppQuery, AppQuery> FirebaseToken
        {
            get
            {
                return c => c.Text("Firebase Token");
            }
        }

        public Func<AppQuery, AppQuery> Logout
        {
            get
            {
                return c => c.Marked("Log out");
            }
        }
    }
}
