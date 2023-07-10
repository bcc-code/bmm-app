using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.Base
{
    public interface ITrackListHolderPO : IDocumentPO
    {
        string Title { get; }

        string Cover { get; }
    }
}