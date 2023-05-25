using System;
using BMM.Core.Models.POs.Tiles;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class VideoTileViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(VideoTileViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(VideoTileViewCell), NSBundle.MainBundle);
        private string _videoUrl;

        public VideoTileViewCell(IntPtr handle): base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<VideoTileViewCell, VideoTilePO>();

                set.Bind(HeaderLabel)
                    .To(vm => vm.Tile.Header);
                
                set.Bind(BottomButton)
                    .For(v => v.BindTitle())
                    .To(vm => vm.Tile.ButtonText);
                
                set.Bind(this)
                    .For(v => v.VideoUrl)
                    .To(vm => vm.Tile.VideoFileName);
                
                set.Bind(BottomButton)
                    .To(vm => vm.BottomButtonClickedCommand);
                
                set.Apply();
            });
        }

        public string VideoUrl
        {
            get => _videoUrl;
            set
            {
                _videoUrl = value;
                string url = NSBundle.MainBundle.PathForResource(_videoUrl, null);
                VideoView.Configure(new NSUrl(url, false), true);
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            HeaderLabel.ApplyTextTheme(AppTheme.Subtitle2Label1);
            HeaderLabel.TextColor = AppColors.LabelOneColor.GetResolvedColorSafe(UIUserInterfaceStyle.Light);
            BottomButton.ApplyButtonStyle(AppTheme.ButtonPrimaryBlackAutoSize);
        }
    }
}