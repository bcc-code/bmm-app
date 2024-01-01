using MvvmCross.Commands;

namespace BMM.Core.Models
{
    public class ProfileListItem : SelectableListItem
    {
        public string Username { get; set; }
        public string UserProfileUrl { get; set; }
        
        public string AchievementsText { get; set; }
        public IMvxCommand LogoutCommand { get; set; }
        
        public IMvxCommand AchievementsClickedCommand { get; set; }
        public IMvxCommand EditProfileCommand { get; set; }
    }
}