using System;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Tiles;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.CollectionViewSource;
using BMM.UI.iOS.Extensions;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ContinueListeningCollectionTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ContinueListeningCollectionTableViewCell));

        public ContinueListeningCollectionTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ContinueListeningCollectionTableViewCell, TileCollectionPO>();

                var source = new TilesCollectionViewSource(ContinueListeningCollection);

                set
                    .Bind(source)
                    .For(s => s.ItemsSource)
                    .To(vm => vm.Tiles);
                
                ContinueListeningCollection!.AccessibilityIdentifier = nameof(ContinueListeningCollection);
                ContinueListeningCollection.Source = source;
                set.Apply();
            });
        }

        protected override bool HasHighlightEffect => false;
    }
}