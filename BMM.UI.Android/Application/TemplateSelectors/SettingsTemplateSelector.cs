using BMM.Core.Models;
using BMM.Core.Models.POs.Other;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class SettingsTemplateSelector : MvxTemplateSelector<IListItem>
    {
        public override int GetItemLayoutId(int fromViewType)
        {
            return fromViewType switch
            {
                SettingsViewTypes.ProfileListItem => Resource.Layout.listitem_profile,
                SettingsViewTypes.CheckboxListItem => Resource.Layout.listitem_checkbox,
                SettingsViewTypes.HeaderListItem => Resource.Layout.listitem_header,
                SettingsViewTypes.ListContentItem => Resource.Layout.listitem_detail,
                _ => -1
            };
        }

        protected override int SelectItemViewType(IListItem forItemObject)
        {
            return forItemObject switch
            {
                ProfileListItem _ => SettingsViewTypes.ProfileListItem,
                CheckboxListItemPO _ => SettingsViewTypes.CheckboxListItem,
                SectionHeader _ => SettingsViewTypes.HeaderListItem,
                SelectableListItem _ => SettingsViewTypes.ListContentItem,
                _ => -1
            };
        }

        private static class SettingsViewTypes
        {
            public const int ProfileListItem = 0;
            public const int CheckboxListItem = 1;
            public const int HeaderListItem = 2;
            public const int ListContentItem = 3;
        }
    }
}