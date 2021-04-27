using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Specialized;
using System.Windows.Input;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Snackbar;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.IoC;
using MvvmCross.Platforms.Android.Views;
using ActionMenuView = AndroidX.AppCompat.Widget.ActionMenuView;
using PopupMenu = AndroidX.AppCompat.Widget.PopupMenu;
using ShareActionProvider = AndroidX.AppCompat.Widget.ShareActionProvider;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

// ReSharper disable UnusedMember.Global
#pragma warning disable 168

namespace BMM.UI.Droid
{
    // This class is never actually executed, but when Xamarin linking is enabled it does ensure types and properties
    // are preserved in the deployed app
    public class LinkerPleaseInclude
    {
        public void Include()
        {
            ActionMenuView menu;
            ShareActionProvider share;
            Toolbar toolbar;
            FitWindowsLinearLayout layout;
            FitWindowsFrameLayout frame;
            AlertDialogLayout alert;
            DialogTitle title;
            ActionBarContainer container;
            ContentFrameLayout content;
            ActionMenuPresenter presenter;
            ActionBarContextView view;
            ActionBarOverlayLayout layout2;
            DialogTitle dialog;
            AlertDialogLayout alertDialogLayout;
            GridLayoutManager manager;
            PopupMenu popup;
            TooltipCompat tooltip;
            Space space;
            Snackbar snackbar;
        }

        public void Include(Button button)
        {
            button.Click += (s, e) => button.Text = button.Text + "";
        }

        public void Include(ImageButton button)
        {
            button.Click += (s, e) => button.Enabled = !button.Enabled;
        }

        public void Include(CheckBox checkBox)
        {
            checkBox.CheckedChange += (sender, args) => checkBox.Checked = !checkBox.Checked;
        }

        public void Include(Switch @switch)
        {
            @switch.CheckedChange += (sender, args) => @switch.Checked = !@switch.Checked;
        }

        public void Include(View view)
        {
            view.Click += (s, e) => view.ContentDescription = view.ContentDescription + "";
        }

        public void Include(TextView text)
        {
            text.TextChanged += (sender, args) => text.Text = "" + text.Text;
            text.Hint = "" + text.Hint;
        }

        public void Include(CheckedTextView text)
        {
            text.TextChanged += (sender, args) => text.Text = "" + text.Text;
            text.Hint = "" + text.Hint;
        }

        public void Include(CompoundButton cb)
        {
            cb.CheckedChange += (sender, args) => cb.Checked = !cb.Checked;
        }

        public void Include(ToggleButton tb)
        {
            tb.CheckedChange += (sender, args) => tb.Checked = !tb.Checked;
        }

        public void Include(Activity act)
        {
            act.Title = act.Title + "";
        }

        public void Include(SeekBar sb)
        {
            sb.ProgressChanged += (sender, args) => sb.Progress = sb.Progress + 1;
            sb.Max = sb.Max + 1;
            sb.SecondaryProgress = sb.SecondaryProgress + 1;
        }

        public void IncludeVisibility(View view)
        {
            view.Visibility = view.Visibility + 1;
        }

        public void Include(LinearLayout layout)
        {
            layout.Visibility = ViewStates.Visible;
        }

        public void Include(MvxActivity act)
        {
            act.Title = act.Title + "";
        }

        public void Include(RecyclerView.ViewHolder vh, MvxRecyclerView list)
        {
            vh.ItemView.Click += (sender, args) => { };
            vh.ItemView.LongClick += (sender, args) => { };
            list.ItemsSource = null;
            list.Click += (sender, args) => { };
        }

        void Include(IMvxRecyclerViewHolder viewHolder)
        {
            void OnItemViewClick(object sender, EventArgs e) { }
            void OnItemViewLongClick(object sender, EventArgs e) { }

            viewHolder.Click -= OnItemViewClick;
            viewHolder.LongClick -= OnItemViewLongClick;
            viewHolder.Click += OnItemViewClick;
            viewHolder.LongClick += OnItemViewLongClick;
        }

        public void Include(INotifyCollectionChanged changed)
        {
            changed.CollectionChanged += (s, e) => { var test = string.Format("{0}{1}{2}{3}{4}", e.Action, e.NewItems, e.NewStartingIndex, e.OldItems, e.OldStartingIndex); };
        }

        public void Include(ICommand command)
        {
            command.CanExecuteChanged += (s, e) => { if (command.CanExecute(null)) command.Execute(null); };
        }

        public void Include(MvxPropertyInjector injector)
        {
            injector = new MvxPropertyInjector();
        }

        public void Include(System.ComponentModel.INotifyPropertyChanged changed)
        {
            changed.PropertyChanged += (sender, e) =>
            {
                var test = e.PropertyName;
            };
        }

        // see following link why this is needed: https://stackoverflow.com/questions/47050608/mvvmcross-5-4-crash-on-app-startup-with-nullref-at-consolelogprovider/47052169#47052169
        // without a reference to the console the linker removes it from the app package
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
    }
}