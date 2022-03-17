using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class FraKaareTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(FraKaareTableViewCell));

        public FraKaareTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FraKaareTableViewCell, CellWrapperViewModel<Document>>();
                
                var podcastSource = new PodcastNewestTrackTableViewSource(PodcastTrackListTableView);
                
                set.Bind(PodcastShowAllButton)
                    .For(v => v.BindTitle())
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).TextSource)
                    .WithConversion<MvxLanguageConverter>(Translations.ExploreNewestViewModel_ShowAll);
                
                set.Bind(PodcastTitleLabel)
                    .For(l => l.Text)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).TextSource)
                    .WithConversion<MvxLanguageConverter>(Translations.ExploreNewestViewModel_FraKaareHeader);
                
                set.Bind(FraKaarePlayRandomButton)
                    .For(v => v.BindTitle())
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).TextSource)
                    .WithConversion<MvxLanguageConverter>(Translations.ExploreNewestViewModel_PlayRandom);
                
                set.Apply();
            });
        }

        protected override bool HasHighlightEffect => false;
    }
}