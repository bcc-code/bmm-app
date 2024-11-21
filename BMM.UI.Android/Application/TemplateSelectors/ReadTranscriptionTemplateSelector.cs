using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Transcriptions;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace BMM.UI.Droid.Application.TemplateSelectors;

public class ReadTranscriptionTemplateSelector : MvxTemplateSelector<IBasePO>
{
    public override int GetItemLayoutId(int fromViewType) => fromViewType;

    protected override int SelectItemViewType(IBasePO forItemObject)
    {
        switch (forItemObject)
        {
            case ReadTranscriptionsPO:
                return Resource.Layout.listitem_read_transcription;
            case TranscriptionHeaderPO:
                return Resource.Layout.listitem_transcription_header;
            default:
                return default;
        }
    }
}