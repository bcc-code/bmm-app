using System;
using Foundation;
using System.Collections.Specialized;
using System.Windows.Input;
using MvvmCross.Binding.BindingContext;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using UIKit;

namespace BMM.UI.iOS
{
    /// <summary>
    /// This class is never actually executed, but when Xamarin linking is enabled it does ensure types and properties
    /// are preserved in the deployed app. See https://www.mvvmcross.com/documentation/fundamentals/linking
    /// </summary>
    [Preserve(AllMembers = true)]
    public class LinkerPleaseInclude
    {
        public void Include2(UIButton uiButton)
        {
            uiButton.TouchUpInside += (s, e) =>
                                      uiButton.SetTitle(uiButton.Title(UIControlState.Normal), UIControlState.Normal);
            uiButton.Selected = uiButton.Selected;
        }

        public void Include2(UIBarButtonItem barButton)
        {
            barButton.Clicked += (s, e) =>
                                 barButton.Title = barButton.Title + "";
        }

        public void Include2(UITextField textField)
        {
            textField.Text = textField.Text + "";
            textField.EditingChanged += (sender, args) => { textField.Text = ""; };
            textField.EditingDidEnd += (sender, args) => { textField.Text = ""; };
        }

        public void Include2(UITextView textView)
        {
            textView.Text = textView.Text + "";
            textView.Changed += (sender, args) => { textView.Text = ""; };
            textView.Ended += (sender, args) => { textView.Text = ""; };
        }

        public void Include2(UILabel label)
        {
            label.Text = label.Text + "";
            label.AttributedText = new NSAttributedString(label.AttributedText.ToString() + "");
        }

        public void Include2(UIImageView imageView)
        {
            imageView.Image = new UIImage(imageView.Image.CGImage);
        }

        public void Include2(UIDatePicker date)
        {
            date.Date = date.Date.AddSeconds(1);
            date.ValueChanged += (sender, args) => { date.Date = NSDate.DistantFuture; };
        }

        public void Include2(UISlider slider)
        {
            slider.Value = slider.Value + 1;
            slider.ValueChanged += (sender, args) => { slider.Value = 1; };
            slider.MaxValue = slider.MaxValue;
        }

        public void Include2(UIProgressView progress)
        {
            progress.Progress = progress.Progress + 1;
        }

        public void Include2(UISwitch sw)
        {
            sw.On = !sw.On;
            sw.ValueChanged += (sender, args) => { sw.On = false; };
        }

        public void Include2(MvxViewController vc)
        {
            vc.Title = vc.Title + "";
        }

        public void Include2(UIStepper s)
        {
            s.Value = s.Value + 1;
            s.ValueChanged += (sender, args) => { s.Value = 0; };
        }

        public void Include(UIScrollView s)
        {
            s.ScrollsToTop = true;
            s.Scrolled += (sender, args) => { s.ScrollsToTop = false; };
        }

        public void Include(UISearchBar s)
        {
            s.Text = s.Text + "";
            s.Placeholder = s.Placeholder + "";
            s.TextChanged += (sender, e) => { };
        }

        public void Include2(INotifyCollectionChanged changed)
        {
            changed.CollectionChanged += (s, e) => { var test = string.Format("{0}{1}{2}{3}{4}", e.Action, e.NewItems, e.NewStartingIndex, e.OldItems, e.OldStartingIndex); };
        }

        public void Include2(ICommand command)
        {
            command.CanExecuteChanged += (s, e) => { if (command.CanExecute(null)) command.Execute(null); };
        }

        public void Include2(MvxPropertyInjector injector)
        {
            injector = new MvxPropertyInjector();
        }

        public void Include2(System.ComponentModel.INotifyPropertyChanged changed)
        {
            changed.PropertyChanged += (sender, e) => { var test = e.PropertyName; };
        }

        // see following link why this is needed: https://stackoverflow.com/questions/47050608/mvvmcross-5-4-crash-on-app-startup-with-nullref-at-consolelogprovider/47052169#47052169
        // without a reference to the console the linker removes it from the app package
        public void Include2(ConsoleColor color)
        {
            Console.Write("");
            Console.WriteLine("");
            color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        // The following content is copied from https://raw.githubusercontent.com/MvvmCross/MvvmCross/master/ContentFiles/iOS/LinkerPleaseInclude.cs
        #region MvvmCross ContentFile

        public void Include(MvxTaskBasedBindingContext c)
        {
            c.Dispose();
            var c2 = new MvxTaskBasedBindingContext();
            c2.Dispose();
        }

        public void Include(UIButton uiButton)
        {
            uiButton.TouchUpInside += (s, e) =>
                                      uiButton.SetTitle(uiButton.Title(UIControlState.Normal), UIControlState.Normal);
        }

        public void Include(UIBarButtonItem barButton)
        {
            barButton.Clicked += (s, e) =>
                                 barButton.Title = barButton.Title + "";
        }

        public void Include(UITextField textField)
        {
            textField.Text = textField.Text + "";
            textField.EditingChanged += (sender, args) => { textField.Text = ""; };
            textField.EditingDidEnd += (sender, args) => { textField.Text = ""; };
        }

        public void Include(UITextView textView)
        {
            textView.Text = textView.Text + "";
            textView.TextStorage.DidProcessEditing += (sender, e) => textView.Text = "";
            textView.Changed += (sender, args) => { textView.Text = ""; };
        }

        public void Include(UILabel label)
        {
            label.Text = label.Text + "";
            label.AttributedText = new NSAttributedString(label.AttributedText.ToString() + "");
        }

        public void Include(UIImageView imageView)
        {
            imageView.Image = new UIImage(imageView.Image.CGImage);
        }

        public void Include(UIDatePicker date)
        {
            date.Date = date.Date.AddSeconds(1);
            date.ValueChanged += (sender, args) => { date.Date = NSDate.DistantFuture; };
        }

        public void Include(UISlider slider)
        {
            slider.Value = slider.Value + 1;
            slider.ValueChanged += (sender, args) => { slider.Value = 1; };
        }

        public void Include(UIProgressView progress)
        {
            progress.Progress = progress.Progress + 1;
        }

        public void Include(UISwitch sw)
        {
            sw.On = !sw.On;
            sw.ValueChanged += (sender, args) => { sw.On = false; };
        }

        public void Include(MvxViewController vc)
        {
            vc.Title = vc.Title + "";
        }

        public void Include(UIStepper s)
        {
            s.Value = s.Value + 1;
            s.ValueChanged += (sender, args) => { s.Value = 0; };
        }

        public void Include(INotifyCollectionChanged changed)
        {
            changed.CollectionChanged += (s, e) => { var test = $"{e.Action}{e.NewItems}{e.NewStartingIndex}{e.OldItems}{e.OldStartingIndex}"; };
        }

        public void Include(ICommand command)
        {
            command.CanExecuteChanged += (s, e) => { if (command.CanExecute(null)) command.Execute(null); };
        }

        public void Include(MvvmCross.IoC.MvxPropertyInjector injector)
        {
            injector = new MvvmCross.IoC.MvxPropertyInjector();
        }

        public void Include(System.ComponentModel.INotifyPropertyChanged changed)
        {
            changed.PropertyChanged += (sender, e) => { var test = e.PropertyName; };
        }

        public void Include(IMvxViewModelLoader loader, IMvxViewDispatcher mvxViewDispatcher, IMvxIoCProvider mvxIoCProvider)
        {
            var service = new MvxNavigationService(loader, mvxViewDispatcher, mvxIoCProvider);
        }

        public void Include(ConsoleColor color)
        {
            Console.Write("");
            Console.WriteLine("");
            color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        #endregion
    }
}