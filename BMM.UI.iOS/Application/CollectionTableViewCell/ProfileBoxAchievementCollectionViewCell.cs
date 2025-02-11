using BMM.Core.Models.POs.BibleStudy.Interfaces;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ProfileBoxAchievementCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new(nameof(ProfileBoxAchievementCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(ProfileBoxAchievementCollectionViewCell), NSBundle.MainBundle);

        public ProfileBoxAchievementCollectionViewCell(ObjCRuntime.NativeHandle handle) : base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ProfileBoxAchievementCollectionViewCell, IAchievementPO>();
                
                set.Bind(ContentView)
                    .For(v => v.BindTap())
                    .To(po => po.AchievementClickedCommand);
                
                set.Bind(ImageView)
                    .For(v => v.ImagePath)
                    .To(po => po.ImagePath);

                set.Apply();
            });
        }
    }
}