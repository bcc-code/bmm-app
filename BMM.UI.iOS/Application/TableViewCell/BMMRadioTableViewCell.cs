using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class BMMRadioTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(BMMRadioTableViewCell));

        public BMMRadioTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<BMMRadioTableViewCell, CellWrapperViewModel<Document>>();

                set.Bind(BmmRadioBroadcastingView)
                    .For(s => s.Hidden)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).RadioViewModel.IsBroadcasting)
                    .WithConversion<InvertedVisibilityConverter>();

                set.Bind(BmmRadioUpcomingView)
                    .For(s => s.Hidden)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).RadioViewModel.IsBroadcastUpcoming)
                    .WithConversion<InvertedVisibilityConverter>();

                set.Bind(BmmRadioPlay)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).RadioViewModel.PlayCommand);
                
                set.Bind(BmmRadioUpcomingPlay)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).RadioViewModel.PlayCommand);
                
                set.Bind(BmmRadioBroadcastingTitle)
                    .For(l => l.Text)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).RadioViewModel.Title)
                    .WithConversion<ToUppercaseConverter>();
                
                set.Bind(BmmRadioUpcomingTitle)
                    .For(l => l.Text)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).RadioViewModel.Title)
                    .WithConversion<ToUppercaseConverter>();
                
                set.Bind(BmmRadioBroadcastingDescription)
                    .For(l => l.Text)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).RadioViewModel.Track.Title);
                
                set.Bind(BmmRadioUpcomingDescription)
                    .For(l => l.Text)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).RadioViewModel.Track.Title);
                
                set.Bind(CountdownLabel)
                    .For(l => l.Text)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).RadioViewModel.TimeLeft)
                    .WithConversion<TimeSpanToCountdownValueConverter>();

                set.Apply();
            });
        }

        protected override bool HasHighlightEffect => false;
    }
}