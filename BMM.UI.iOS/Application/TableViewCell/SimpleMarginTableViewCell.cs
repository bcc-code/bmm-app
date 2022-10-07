using Foundation;
using System;

namespace BMM.UI.iOS
{
    public partial class SimpleMarginTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(SimpleMarginTableViewCell));

        public SimpleMarginTableViewCell(IntPtr handle)
            : base(handle)
        {
        }

        protected override bool HasHighlightEffect => false;
    }
}