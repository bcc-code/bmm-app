﻿namespace BMM.UI.Droid.Application.TemplateSelectors
{
    public class ExploreNewestTemplateSelector : DocumentTemplateSelector
    {
		public override int GetItemLayoutId(int fromViewType)
		{
            if (fromViewType == ViewTypes.FraKaareTeaser)
            {
                return Resource.Layout.listitem_fra_kaare_teaser;
            }
            if (fromViewType == ViewTypes.LiveRadio)
            {
                return Resource.Layout.listitem_live_radio;
            }
            if (fromViewType == ViewTypes.AslaksenTeaser)
            {
                return Resource.Layout.listitem_aslaksen_teaser;
            }

            return base.GetItemLayoutId(fromViewType);
		}
    }
}
