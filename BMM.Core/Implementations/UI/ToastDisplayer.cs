using System;
using System.Drawing;
using System.Threading.Tasks;
using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.Base;

namespace BMM.Core.Implementations.UI
{
    public class ToastDisplayer: IToastDisplayer
    {
        private const int ToastDuration = 4000;
        private readonly IUserDialogs _userDialogs;

        public ToastDisplayer(IUserDialogs userDialogs)
        {
            _userDialogs = userDialogs;
        }

        private static ToastConfig CreateToastConfig(string message, Color backgroundColor)
        {
            return new ToastConfig(message)
            {
                Duration = new TimeSpan(ToastDuration * TimeSpan.TicksPerMillisecond),
                MessageTextColor = Color.FromArgb(255, 255, 255),
                BackgroundColor = backgroundColor
            };
        }

        public Task Success(string message)
        {
            return Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>().ExecuteOnMainThreadAsync(() =>
            {
                _userDialogs.Toast(CreateToastConfig(message, Color.FromArgb(131, 231, 107)));
            });
        }

        public void Warn(string message)
        {
            Mvx.IoCProvider.Resolve<IMvxMainThreadDispatcher>().RequestMainThreadAction(() =>
            {
                _userDialogs.Toast(CreateToastConfig(message, Color.FromArgb(255, 183, 49)));
            });
        }

        public Task WarnAsync(string message)
        {
            return Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>().ExecuteOnMainThreadAsync(() =>
            {
                _userDialogs.Toast(CreateToastConfig(message, Color.FromArgb(255, 183, 49)));
            });
        }

        public void Error(string message)
        {
            Mvx.IoCProvider.Resolve<IMvxMainThreadDispatcher>().RequestMainThreadAction(() =>
            {
                _userDialogs.Toast(CreateToastConfig(message, Color.FromArgb(254, 67, 101)));
            });
        }

        public Task ErrorAsync(string message)
        {
            return Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>().ExecuteOnMainThreadAsync(() =>
            {
                _userDialogs.Toast(CreateToastConfig(message, Color.FromArgb(254, 67, 101)));
            });
        }
    }
}