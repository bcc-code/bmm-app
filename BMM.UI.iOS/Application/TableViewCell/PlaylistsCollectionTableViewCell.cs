using System;
using BMM.Api.Implementation.Models;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Extensions;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class PlaylistsCollectionTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(PlaylistsCollectionTableViewCell));

        public PlaylistsCollectionTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<PlaylistsCollectionTableViewCell, CellWrapperViewModel<CoverCarouselCollection>>();

                var source = new MvxCollectionViewSource(PlaylistsCollectionView, CoverWithTitleCollectionViewCell.Key);
                PlaylistsCollectionView!.RegisterNibForCell(CoverWithTitleCollectionViewCell.Nib, CoverWithTitleCollectionViewCell.Key);

                set
                    .Bind(source)
                    .For(s => s.ItemsSource)
                    .To(vm => vm.Item.CoverDocuments)
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

        public nfloat CollectionViewOffset
        {
            get => PlaylistsCollectionView.ContentOffset.X;
            set => PlaylistsCollectionView.SetXOffset(value, false);
        }

        private CellWrapperViewModel<Document> CellDataContext => (CellWrapperViewModel<Document>)DataContext;
    }
}