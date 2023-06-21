using System;
using BMM.Core.ViewModels;

namespace BMM.UI.iOS
{
    public partial class TrackInfoViewController : BaseViewController<TrackInfoViewModel>
    {
        public TrackInfoViewController() : base(nameof(TrackInfoViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new TrackInfoTableViewSource(TrackInfoTableView);
            source.Sections = ViewModel.Sections;
            source.SelectionChangedCommand = ViewModel.ItemSelectedCommand;
        }
    }
}