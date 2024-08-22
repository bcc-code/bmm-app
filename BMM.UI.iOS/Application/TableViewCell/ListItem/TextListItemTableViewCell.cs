﻿using System;
using BMM.Core.Models;
using BMM.Core.Models.POs.Other.Interfaces;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class TextListItemTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(TextListItemTableViewCell));

        public TextListItemTableViewCell(ObjCRuntime.NativeHandle handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<TextListItemTableViewCell, ISelectableListContentItemPO>();
                set.Bind(TitleLabel).To(listItem => listItem.Title);
                set.Bind(TextLabel).To(listItem => listItem.Text);
                set.Apply();
            });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
            TextLabel.ApplyTextTheme(AppTheme.Subtitle3Label2);
        }
    }
}