using System;
using BMM.Core.Models.POs.Tiles;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class MessageTileViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(MessageTileViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(MessageTileViewCell), NSBundle.MainBundle);

        public MessageTileViewCell(IntPtr handle): base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<MessageTileViewCell, MessageTilePO>();

                set.Bind(HeaderLabel)
                    .To(vm => vm.Tile.Header);
                
                set.Bind(TitleLabel)
                    .To(vm => vm.Tile.Title);
                
                set.Bind(SubtitleLabel)
                    .To(vm => vm.Tile.Subtitle);
                
                set.Bind(BottomButton)
                    .For(v => v.BindTitle())
                    .To(vm => vm.Tile.CloseButtonText);
                
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Heading3);
            HeaderLabel.ApplyTextTheme(AppTheme.Subtitle2Label1);
            SubtitleLabel.ApplyTextTheme(AppTheme.Subtitle2Label1);
            BottomButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
        }
    }
}