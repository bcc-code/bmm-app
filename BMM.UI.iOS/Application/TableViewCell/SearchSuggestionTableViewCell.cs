using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class SearchSuggestionTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(SearchSuggestionTableViewCell));

        public SearchSuggestionTableViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<SearchSuggestionTableViewCell, string>();
                    set.Bind(TextLabel);
                    set.Apply();
                });
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TextLabel.ApplyTextTheme(AppTheme.Title2);
        }
    }
}