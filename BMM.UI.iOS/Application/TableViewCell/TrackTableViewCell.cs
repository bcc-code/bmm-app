using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Helpers;
using MvvmCross.Platforms.Ios.Binding;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class TrackTableViewCell : BaseTrackTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("TrackTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("TrackTableViewCell");

        private VisibilityBindingsManager<CellWrapperViewModel<Document>> _bindingsManager;

        public TrackTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<TrackTableViewCell, CellWrapperViewModel<Track>>();
                set.Bind(TitleLabel).WithConversion<DocumentToTitleValueConverter>();
                set.Bind(TitleLabel)
                    .For(i => i.TextColor)
                    .To(vm => ((DocumentsViewModel)vm.ViewModel).CurrentTrack)
                    .WithConversion<TrackToTitleColorConverter>((Func<CellWrapperViewModel<Document>>) (() => (CellWrapperViewModel<Document>)DataContext));
                set.Bind(accessoryView).WithConversion<DocumentToSubtitleValueConverter>();
                set.Bind(accessoryView).For(i => i.TextColor).WithConversion<TrackToSubtitleColorConverter>();
                set.Bind(metaLabel).WithConversion<DocumentToMetaValueConverter>();
                set.Bind(metaLabel).For(l => l.TextColor).WithConversion<TrackToMetaColorConverter>();
                set.Bind(DownloadStatusImageView).For(i => i.ImagePath).To(vm => vm).WithConversion<OfflineAvailableTrackStatusConverter>();
                set.Bind(DownloadStatusImageView).For(i => i.BindVisibility()).To(vm => vm).WithConversion<OfflineAvailableTrackValueConverter>();
                set.Bind(StatusImage)
                    .For(v => v.Image)
                    .To(vm => ((DocumentsViewModel)vm.ViewModel).CurrentTrack)
                    .WithConversion<TrackToStatusImageConverter>((Func<CellWrapperViewModel<Document>>) (() => (CellWrapperViewModel<Document>)DataContext));
                set.Bind(OptionsButton).WithConversion<OptionButtonCommandValueConverter>();
                set.Bind(ReferenceButton).WithConversion<ShowTrackInfoCommandValueConverter>();
                set.Apply();
            });

            BindingContext.DataContextChanged += (sender, e) =>
            {
                if (DataContext == null)
                {
                    return;
                }

                if (_bindingsManager == null)
                {
                    _bindingsManager = new VisibilityBindingsManager<CellWrapperViewModel<Document>>();
                    _bindingsManager.AddBinding(DownloadStatusImageView, DownloadStatusVisible);
                    _bindingsManager.AddBinding(ReferenceButton, ReferenceButtonVisible);
                }

                _bindingsManager.Update((CellWrapperViewModel<Document>) DataContext);
            };
        }

        public static TrackTableViewCell Create()
        {
            return (TrackTableViewCell)Nib.Instantiate(null, null)[0];
        }
    }
}