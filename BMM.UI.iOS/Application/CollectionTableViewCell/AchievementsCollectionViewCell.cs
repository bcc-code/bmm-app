using BMM.Core.Constants;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.UI.iOS.Extensions;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class AchievementsCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new(nameof(AchievementsCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(AchievementsCollectionViewCell), NSBundle.MainBundle);
        private string _imagePath;

        public AchievementsCollectionViewCell(IntPtr handle) : base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<AchievementsCollectionViewCell, IAchievementPO>();
                
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