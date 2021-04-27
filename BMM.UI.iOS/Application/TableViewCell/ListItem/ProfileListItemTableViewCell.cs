using System;
using BMM.Core.Models;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Helpers;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ProfileListItemTableViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString("ProfileListItemTableViewCell");

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
                set.Bind(SignOutButton).For("Title").To(listItem => listItem.Text);
                set.Bind(SignOutButton).To(listItem => listItem.LogoutCommand);

                set.Apply();
            });
        }
    }
}