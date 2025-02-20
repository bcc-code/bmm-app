using BMM.Core.Models.POs.BibleStudy.Interfaces;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class ProjectBoxAchievementCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new(nameof(ProjectBoxAchievementCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(ProjectBoxAchievementCollectionViewCell), NSBundle.MainBundle);

        public ProjectBoxAchievementCollectionViewCell(ObjCRuntime.NativeHandle handle) : base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ProjectBoxAchievementCollectionViewCell, IAchievementPO>();
                
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