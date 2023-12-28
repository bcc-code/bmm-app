using System;
using BMM.Core.Models;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Helpers;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ProfileListItemTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ProfileListItemTableViewCell));

        public ProfileListItemTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ProfileListItemTableViewCell, ProfileListItem>();
                set.Bind(SignedInAsTitle).To(listItem => listItem.Title);
                set.Bind(Username).To(listItem => listItem.Username);
                
                set.Bind(ProfileImage)
                    .For(v => v.ImagePath)
                    .To(listItem => listItem.UserProfileUrl)
                    .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.ArtistPlaceholderImage);
                set.Bind(ProfileImage).For(v => v.BindTap()).To(listItem => listItem.EditProfileCommand);
                set.Bind(SignOutButton).For(v => v.BindTitle()).To(listItem => listItem.Text);
                set.Bind(SignOutButton).To(listItem => listItem.LogoutCommand);
                set.Bind(AchievementsButton)
                    .For(v => v.BindTitle())
                    .To(l => l.AchievementsText);
                
                set.Bind(AchievementsButton)
                    .To(l => l.AchievementsClickedCommand);

                set.Apply();
            });
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SignedInAsTitle.ApplyTextTheme(AppTheme.Subtitle2Label3);
            Username.ApplyTextTheme(AppTheme.Title1);
        }
    }
}