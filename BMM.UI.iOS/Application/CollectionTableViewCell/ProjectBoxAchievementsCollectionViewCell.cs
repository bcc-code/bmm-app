using BMM.Core.Constants;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.UI.iOS.Extensions;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ProjectBoxAchievementsCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new(nameof(ProjectBoxAchievementsCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(ProjectBoxAchievementsCollectionViewCell), NSBundle.MainBundle);

        public ProjectBoxAchievementsCollectionViewCell(ObjCRuntime.NativeHandle handle) : base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ProjectBoxAchievementsCollectionViewCell, IAchievementPO>();
                
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