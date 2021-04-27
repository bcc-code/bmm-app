using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using Foundation;
using System;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class SearchSuggestionTableViewCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("SearchSuggestionTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("SearchSuggestionTableViewCell");

        public SearchSuggestionTableViewCell(IntPtr handle) : base(handle)
        {
            this.DelayBind(() =>
                {
                    var set = this.CreateBindingSet<SearchSuggestionTableViewCell, string>();
                    set.Bind(TextLabel);
                    set.Apply();
                });
        }

        public static SearchSuggestionTableViewCell Create()
        {
            return (SearchSuggestionTableViewCell)Nib.Instantiate(null, null)[0];
        }
    }
}