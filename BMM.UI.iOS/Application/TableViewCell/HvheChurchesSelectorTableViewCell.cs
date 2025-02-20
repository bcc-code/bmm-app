using System;
using System.ComponentModel;
using System.Drawing;
using BMM.Core.Constants;
using BMM.Core.Models.POs.BibleStudy;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using ObjCRuntime;
using UIKit;

namespace BMM.UI.iOS.CustomViews
{
    public partial class HvheChurchesSelectorTableViewCell : BaseBMMTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName(nameof(HvheChurchesSelectorTableViewCell), NSBundle.MainBundle);
        public static readonly NSString Key = new(nameof(HvheChurchesSelectorTableViewCell));

        public HvheChurchesSelectorTableViewCell(NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(Bind);
        }

        protected override bool HasHighlightEffect => false;

        private void Bind()
        {
            var set = this.CreateBindingSet<HvheChurchesSelectorTableViewCell, HvheChurchesSelectorPO>();

            set.Bind(ChurchesSelectorView)
                .For(v => v.DataContext)
                .To(po => po);
            
            set.Apply();
        }
    }
}