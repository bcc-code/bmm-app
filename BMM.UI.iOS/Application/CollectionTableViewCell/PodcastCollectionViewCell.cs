﻿using System;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class PodcastCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new NSString("PodcastCollectionViewCell");
        public static readonly UINib Nib = UINib.FromName("PodcastCollectionViewCell", NSBundle.MainBundle);

        public PodcastCollectionViewCell(ObjCRuntime.NativeHandle handle): base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<PodcastCollectionViewCell, ITrackListHolderPO>();

                set.Bind(TitleLabel).To(vm => vm.Title);
                ImageView.ErrorAndLoadingPlaceholderImagePathForCover();
                set.Bind(ImageView)
                    .For(v => v.ImagePath)
                    .To(vm => vm.Cover)
                    .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.CoverPlaceholderImage);

                set.Apply();
            });
        }
    }
}
