using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.Models.POs;
using BMM.UI.iOS.Constants;
using CoreAnimation;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class AppIconTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(AppIconTableViewCell));
        private string _imagePath;

        public AppIconTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<AppIconTableViewCell, AppIconPO>();

                set.Bind(TextLabel)
                    .To(vm => vm.Name);

                set.Bind(this)
                    .For(v => v.ImagePath)
                    .To(vm => vm.ImagePath);

                set.Bind(IsSelectedImage)
                    .For(v => v.BindVisible())
                    .To(vm => vm.IsSelected);

                set.Apply();
            });
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                IconImageView.Image = UIImage.FromBundle(_imagePath);
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TextLabel.ApplyTextTheme(AppTheme.Title2);
            IsSelectedImage.TintColor = AppColors.LabelOneColor;
        }
    }
}