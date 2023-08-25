using BMM.Core.Constants;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Presenters;
using MvvmCross.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace BMM.UI.iOS
{
    public partial class BibleStudyRulesViewController
        : BaseViewController<BibleStudyRulesViewModel>,
          IHaveLargeTitle
    {
        private string _pageTitle;

        public BibleStudyRulesViewController() : base(null)
        {
        }

        public BibleStudyRulesViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            var set = this.CreateBindingSet<BibleStudyRulesViewController, BibleStudyRulesViewModel>();

            var source = new BibleStudyRulesTableViewSource(RulesTableView);
            
            set.Bind(source)
                .To(vm => vm.Items);

            set.Bind(this)
                .For(v => v.PageTitle)
                .To(vm => vm.Title);

            set.Apply();
        }

        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                Title = _pageTitle;
            }
        }

        public double? InitialLargeTitleHeight { get; set; }
    }
}