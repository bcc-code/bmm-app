using System;
using BMM.Api.Implementation.Models;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class PlaylistsCollectionTableViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(PlaylistsCollectionTableViewCell));

        public PlaylistsCollectionTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<PlaylistsCollectionTableViewCell, CellWrapperViewModel<PlaylistsCollection>>();

                var source = new MvxCollectionViewSource(PlaylistsCollectionView, CoverWithTitleCollectionViewCell.Key);
                PlaylistsCollectionView.RegisterNibForCell(CoverWithTitleCollectionViewCell.Nib, CoverWithTitleCollectionViewCell.Key);

                set
                    .Bind(source)
                    .For(s => s.ItemsSource)
                    .To(vm => vm.Item.Playlists)
                    .WithConversion<DocumentListValueConverter>(CellDataContext.ViewModel);

                 set
                     .Bind(source)
                     .For(s => s.SelectionChangedCommand)
                     .To(vm => vm.ViewModel.DocumentSelectedCommand)
                     .WithConversion(new DocumentSelectedCommandValueConverter());

                PlaylistsCollectionView.Source = source;
                set.Apply();
            });
        }

        private CellWrapperViewModel<Document> CellDataContext => (CellWrapperViewModel<Document>)DataContext;
    }
}