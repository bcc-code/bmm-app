using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class PlaybackHistoryViewController : BaseViewController<PlaybackHistoryViewModel>
    {
        public PlaybackHistoryViewController()
            : base(nameof(PlaybackHistoryViewController))
        {
        }

        public override System.Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetThemes();

            var source = new DocumentsTableViewSource(PlaybackHistoryTableView);

            var set = this.CreateBindingSet<PlaybackHistoryViewController, PlaybackHistoryViewModel>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand)
                .WithConversion<DocumentSelectedCommandValueConverter>();

            set.Bind(NoEntriesLabelTitle)
                .To(vm => vm.TextSource[Translations.PlaybackHistoryViewModel_NoHistoryYetTitle]);

            set.Bind(NoEntriesLabelSubtitle)
                .To(vm => vm.TextSource[Translations.PlaybackHistoryViewModel_NoHistoryYetSubtitle]);

            set.Bind(NoEntriesLabelTitle)
                .For(v => v.BindVisible())
                .To(vm => vm.HasAnyEntry)
                .WithConversion<InvertedBoolConverter>();

            set.Bind(NoEntriesLabelSubtitle)
                .For(v => v.BindVisible())
                .To(vm => vm.HasAnyEntry)
                .WithConversion<InvertedBoolConverter>();

            set.Bind(PlaybackHistoryTableView)
                .For(v => v.BindVisible())
                .To(vm => vm.HasAnyEntry);

            set.Apply();
        }

        private void SetThemes()
        {
            NoEntriesLabelTitle.ApplyTextTheme(AppTheme.Heading3);
            NoEntriesLabelSubtitle.ApplyTextTheme(AppTheme.Paragraph1Label2);
        }
    }
}