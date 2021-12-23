using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class AslaksenTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(AslaksenTableViewCell));

        public AslaksenTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<AslaksenTableViewCell, CellWrapperViewModel<Document>>();
                
                set.Bind(AslaksenShowAllButton)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).AslaksenTeaserViewModel.ShowAllCommand);
                
                set.Bind(AslaksenShowAllButton)
                    .For(v => v.BindTitle())
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).TextSource)
                    .WithConversion<MvxLanguageConverter>(Translations.ExploreNewestViewModel_ShowAll);
                
                set.Bind(AslaksenTitle)
                    .For(l => l.Text)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).TextSource)
                    .WithConversion<MvxLanguageConverter>(Translations.ExploreNewestViewModel_AslaksenTeaserHeader);
                
                set.Bind(AslaksenPlayRandomButton)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).AslaksenTeaserViewModel.PlayRandomCommand);
                
                set.Bind(AslaksenPlayRandomButton)
                    .For(v => v.BindTitle())
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).TextSource)
                    .WithConversion<MvxLanguageConverter>(Translations.ExploreNewestViewModel_PlayRandom);
                
                set.Bind(AslaksenPlayNewestButton)
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).AslaksenTeaserViewModel.PlayNewestCommand);
                
                set.Bind(AslaksenPlayNewestButton)
                    .For(v => v.BindTitle())
                    .To(vm => ((ExploreNewestViewModel)vm.ViewModel).TextSource)
                    .WithConversion<MvxLanguageConverter>(Translations.ExploreNewestViewModel_PlayNewest);
                
                set.Apply();
            });
        }

        protected override bool HasHighlightEffect => false;
    }
}