using System;
using BMM.Core;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;

namespace BMM.UI.iOS
{
    public partial class AutomaticDownloadViewController : BaseViewController<AutomaticDownloadViewModel>
    {
        public AutomaticDownloadViewController() : base(nameof(AutomaticDownloadViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
        	base.ViewDidLoad();

            var source = new BaseSimpleTableViewSource(AutomaticDownloadTableView, AutomaticDownloadTableViewCell.Key);
        	AutomaticDownloadTableView.RowHeight = 55;
        	AutomaticDownloadTableView.Source = source;

            source.ItemsSource = ViewModel.DownloadOptions;

        	var set = this.CreateBindingSet<AutomaticDownloadViewController, AutomaticDownloadViewModel>();
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DownloadOptionsSelectedCommand)
                .WithConversion(new DownloadOptionSelectedCommandValueConverter());
            set.Bind(SubtitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.AutomaticDownloadViewModel_AutomaticDownloadSubtitle);
        	set.Apply();

        	AutomaticDownloadTableView.ReloadData();
        }
    }
}
